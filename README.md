# BetterWithDona
Here's my version of Window Site to present my résume.
 Access the app through https://betterwithdona.herokuapp.com

Example of appsettings.json
```
{
  "ConnectionStrings": {
    // this could be used in future
    "DefaultConnection": "YOUR CONNECTION STRING"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "ResumeId": "YOUR RESUME ID",
  "path": "./",
  //my ethereal SMTP TEST
  "ServerMail": {
    "smtp": "smtp.ethereal.email",
    "userSmtp": "lee.casper@ethereal.email",
    "pwdSmtp": "pwdSmtp",
    "port": "587"
  },
  "ContentfulOptions": {
    "DeliveryApiKey": "DeliveryApiKey",
    "ManagementApiKey": "ManagementApiKey",
    "PreviewApiKey": "PreviewApiKey",
    "SpaceId": "SpaceId",
    "UsePreviewApi": false,
    "MaxNumberOfRateLimitRetries": 0
  },
  "fakeUser": "fakeUser",
  "fakePwd": "fakePwd"
}

```


# Job Frontend Homework

As part of our application process, we'd like to see what you can produce by giving you a small assignment. It should take you no more than a few hours to complete the assignment, but any extra polish or features you might want to put in will not go unnoticed.

## The assignment

We would like you to create an website application to show your CV . The features it should include:

 - [ x ] Show your CV to the visitor.
 - [ x ] Allow to visitors to send you offers via a form (if the forms sends also an email great!)
 - [ x ] Have a login area, even if with no credentials and mocked up.
 - [ ] In the private area (after the login) allow you to manage your CV and add, delete, update positions and personal data (shouldn't be working but just reproduce the user experience).

The content part should be integrated via a headless CMS like Contentful. 

#### Extra credit features

 - [ ] Put the project online using Heroku or similar
 - [ ] Polish and UX
 - [ ] Highly reusable components
 - [ ] Tests
