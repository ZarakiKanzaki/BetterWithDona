using BetterWithDona.Models;
using Contentful.Core;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contentful.Core.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BetterWithDona.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IContentfulClient client;
        private readonly IContentfulManagementClient managementClient;
        private readonly IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        public HomeController(ILogger<HomeController> logger, IContentfulClient client, IContentfulManagementClient managementClient)
        {
            this.logger = logger;
            this.client = client;
            this.managementClient = managementClient;
        }

        public async Task<IActionResult> Index()
        {
            string resumeId = config.GetValue<string>("ResumeId");
            var builder = new QueryBuilder<Resume>().FieldEquals(a => a.Sys.Id, resumeId).Include(3);
            var resume = (await client.GetEntries(builder)).FirstOrDefault();
            return View(resume);
        }

        [HttpPost]
        public async Task<IActionResult> Index(WorkOffer workOffer, IFormFile file)
        {
            string resumeId = config.GetValue<string>("ResumeId");
            var builder = new QueryBuilder<Resume>().FieldEquals(a => a.Sys.Id, resumeId).Include(3);
            var resume = (await client.GetEntries(builder)).FirstOrDefault();
            var wrk=await managementClient.GetContentType("workOffer");
            if(wrk==null)
            { 
               await CreateWorkOffer();
            }
            try
            {
                if (file.Length > 0)
                {
                    await ManageUploadedFile(workOffer, file);
                    await managementClient.CreateOrUpdateEntry(workOffer, id: workOffer.Email.Replace('.', '_'), version: 1);
                    sendEmailToUser(workOffer.Email, workOffer, resume);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.errMessage ="OPS! Something went wrong, try again."; 
            }

            return View(resume);
        }

        private async Task ManageUploadedFile(WorkOffer workOffer, IFormFile file)
        {
            if (!System.IO.Directory.Exists("./temp"))
            {
                System.IO.Directory.CreateDirectory("./temp");
            }
            using (var stream = System.IO.File.Create("./temp/" + file.FileName))
            {
                await file.CopyToAsync(stream);
            }
            var bytes = System.IO.File.ReadAllBytes("./temp/" + file.FileName);

            var managementAsset = new ManagementAsset
            {
                SystemProperties = new SystemProperties { Id = workOffer.Email.Replace('.', '_') },
                Title = new Dictionary<string, string> {
                            { "en-US", workOffer.Email }
                        },
                Files = new Dictionary<string, File>
                        {
                            { "en-US", new File() {
                                    ContentType = "text/plain",
                                    FileName = file.FileName
                                }
                            }
                        }
            };

            await managementClient.UploadFileAndCreateAsset(managementAsset, bytes);
            workOffer.Offer = await client.GetAsset(workOffer.Email.Replace('.', '_'));
        }

        private async Task CreateWorkOffer()
        {
            var contentType = new ContentType{ 
                SystemProperties = new SystemProperties(),
                Name = "WorkOffer" 
            };
            contentType.SystemProperties.Id = "workOffer";
            contentType.Fields = new List<Field>()
            {
                new Field()
                {
                    Name = "Name",
                    Id = "name",
                    Type = "Text",
                    Required = true,
                    Localized = false,
                    Disabled = false,
                    Omitted = false,
                },
                 new Field()
                {
                    Name = "CompanyName",
                    Id = "companyName",
                    Type = "Text",
                    Required = true,
                    Localized = false,
                    Disabled = false,
                    Omitted = false,
                },
                new Field()
                {
                    Name = "Message",
                    Id = "message",
                    Type = "Text",
                    Required = true,
                    Localized = false,
                    Disabled = false,
                    Omitted = false,
                },
                new Field()
                {
                    Name = "Email",
                    Id = "email",
                    Type = "Symbol",
                    Required = false,
                    Localized = true,
                    Disabled = false,
                    Omitted = false,
                    Validations = new List<IFieldValidator>() {
                        new RegexValidator(expression: "^\\w[\\w.-]*@([\\w-]+\\.)+[\\w-]+$",  flags:"i", message: "The value isn't an e-mail address.")
                    }
                },

                new Field()
                {
                    Name = "Offer",
                    Id = "offer",
                    Type = "Asset",
                    Required = false,
                    Localized = false,
                    Disabled = false,
                    Omitted = false,
                    LinkType = "Asset",
                    Validations = new List<IFieldValidator>() {
                        new MimeTypeValidator(new List<MimeTypeRestriction>(){ MimeTypeRestriction.PdfDocument }, "There's not a PDF here")
                    }
                },
            };
            await managementClient.CreateOrUpdateContentType(contentType);
        }


        private void sendEmailToUser(string email, WorkOffer workOffer, Resume resume)
        {

            string root = config.GetValue<string>("path");
            MailMessage message = new MailMessage(new MailAddress("noreply@betterwithdona.com", "Davide Donadio - Better With Dona"), new MailAddress(email));
            //MailMessage message = new MailMessage("noreply@laquindicesima.it", email);
            string mailTemplate = System.IO.File.ReadAllText(root + "/DefaultDocuments/MailTemplate/notifica_mail.html");
            message.Subject = "Thank you for Submitting your Request";
            mailTemplate = mailTemplate.Replace("%NAME%", workOffer.Name);
            message.Body = mailTemplate;
            
            message.IsBodyHtml = true;

            SmtpClient client = new SmtpClient(config.GetValue<string>("smtp"))
            {
                Credentials = new NetworkCredential(config.GetValue<string>("userSmtp"), config.GetValue<string>("pwdSmtp")),
                Port = 465,
                EnableSsl = true
            };
            client.Send(message);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
