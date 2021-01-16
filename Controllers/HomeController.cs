using BetterWithDona.Models;
using Contentful.Core;
using Contentful.Core.Errors;
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
        public async Task<IActionResult> Index(WorkOffer workOffer, IFormFile fileUploaded)
        {
            string resumeId = config.GetValue<string>("ResumeId");
            var builder = new QueryBuilder<Resume>().FieldEquals(a => a.Sys.Id, resumeId).Include(3);
            var resume = (await client.GetEntries(builder)).FirstOrDefault();

            try
            {
                //  didn't find any docs about checking the existence of a content Type, fix this on i the later time
                var wrk = await managementClient.GetContentType("workOffer");
                try
                {
                    if (fileUploaded.Length > 0)
                    {
                        await ManageUploadedFile(workOffer, fileUploaded);
                        await HandleOffer(workOffer);
                        await sendEmailToUser(workOffer.Email, workOffer, resume);
                        TempData["message"] = "Message submitted with success.";
                        return await Task.Run<ActionResult>(() =>
                        {
                            return RedirectToAction("Index", "Home");
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ViewBag.ErrMessage = "OPS! Something went wrong, try again.";
                }

            }
            catch (ContentfulException ce)
            {
                await CreateWorkOffer();

            }

            return View(resume);
        }

        private async Task HandleOffer(WorkOffer workOffer)
        {
            string offerId = "offer-" + workOffer.Email.Replace('.', '-').Replace('@', '-');
            //check the existence of the entry
            try
            {
                var assetFound = await managementClient.GetEntry(offerId);
                if (assetFound.SystemProperties.PublishedVersion != null)
                {
                    await managementClient.UnpublishEntry(offerId, assetFound.SystemProperties.Version ?? 1);
                }
                await managementClient.DeleteEntry(offerId, assetFound.SystemProperties.Version ?? 1);
            }
            catch (ContentfulException)
            {
                //do nothing, the entry doesn't existst and it's ok
            }
            var entry = new Entry<dynamic>
            {
                SystemProperties = new SystemProperties
                {
                    Id = offerId,

                },
                Fields = new
                {
                    Name = new Dictionary<string, string>()
                                {
                                    { "en-US", workOffer.Name },
                                },
                    Email = new Dictionary<string, string>()
                                {
                                    { "en-US", workOffer.Email },
                                },
                    Message = new Dictionary<string, string>()
                                {
                                    { "en-US", workOffer.Message },
                                },
                    CompanyName = new Dictionary<string, string>()
                                {
                                    { "en-US", workOffer.CompanyName },
                                },
                    Offer = new Dictionary<string, dynamic>()
                                {
                                    { "en-US", new { sys = new { type = "Link", linkType = "Asset", id = workOffer.Offer.SystemProperties.Id} } }
                                },

                }
            };
            await managementClient.CreateOrUpdateEntry(entry, contentTypeId: "workOffer");
            await managementClient.PublishEntry(offerId, version: 1);
        }

        private async Task ManageUploadedFile(WorkOffer workOffer, IFormFile file)
        {
            string Id = "file-" + workOffer.Email.Replace('.', '-').Replace('@', '-');
            if (!System.IO.Directory.Exists("./temp"))
            {
                System.IO.Directory.CreateDirectory("./temp");
            }
            using (var stream = System.IO.File.Create("./temp/" + file.FileName))
            {
                await file.CopyToAsync(stream);
            }
            var bytes = System.IO.File.ReadAllBytes("./temp/" + file.FileName);
            //check the existence of the asset
            try
            {
                var assetFound = await managementClient.GetAsset(Id);
                if (assetFound.SystemProperties.PublishedVersion != null)
                {
                    await managementClient.UnpublishAsset(Id, assetFound.SystemProperties.Version ?? 1);
                }
                await managementClient.DeleteAsset(Id, assetFound.SystemProperties.Version ?? 1);
            }
            catch (ContentfulException)
            {
                //do nothing, the asset doesn't existst and it's ok
            }


            var managementAsset = new ManagementAsset
            {
                SystemProperties = new SystemProperties { Id = Id },
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
            await managementClient.PublishAsset(Id, 2);

            workOffer.Offer = await client.GetAsset(Id);
        }

        private async Task CreateWorkOffer()
        {
            var contentType = new ContentType
            {
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
                    Type = "Text",
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
                    Type = "Object",
                    Required = false,
                    Localized = false,
                    Disabled = false,
                    Omitted = false,
                    LinkType = "Asset",
                    // Some anomalies with the SDK
                    //Validations = new List<IFieldValidator>() {
                    //    new MimeTypeValidator(new List<MimeTypeRestriction>(){ MimeTypeRestriction.PdfDocument }, "There's not a PDF here")
                    //}
                },
            };
            await managementClient.CreateOrUpdateContentType(contentType);
        }


        private async Task sendEmailToUser(string email, WorkOffer workOffer, Resume resume)
        {

            string root = config.GetValue<string>("path");
            MailMessage message = new MailMessage(new MailAddress("noreply@betterwithdona.com", "Davide Donadio - Better With Dona"), new MailAddress(email));
            //MailMessage message = new MailMessage("noreply@laquindicesima.it", email);
            string mailTemplate = System.IO.File.ReadAllText(root + "/DefaultDocuments/MailTemplate/notifica_mail.html");
            message.Subject = "Thank you for Submitting your Request";
            mailTemplate = mailTemplate.Replace("%NAME%", workOffer.Name);
            mailTemplate = mailTemplate.Replace("%CONTACT%", resume.Contacts.FirstOrDefault(a => a.ContactType == "email").Contact);
            mailTemplate = mailTemplate.Replace("%LINKEDIN%", resume.UsefulLinks.FirstOrDefault(a => a.EntryType == "linkedin").Name);
            message.Body = mailTemplate;

            message.IsBodyHtml = true;

            using (SmtpClient smtp = new SmtpClient(config.GetValue<string>("smtp"), 587))
            {
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(config.GetValue<string>("userSmtp"), config.GetValue<string>("pwdSmtp"));
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);
            }
            //SmtpClient client = new SmtpClient(config.GetValue<string>("smtp"))
            //{
            //    Credentials = new NetworkCredential(config.GetValue<string>("userSmtp"), config.GetValue<string>("pwdSmtp")),
            //    Port = 587,
            //    EnableSsl = true
            //};
            //client.Send(message);
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
