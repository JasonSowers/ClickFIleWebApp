using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using System.Web;


namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        private Uri _baseUri;
        public EchoDialog(Uri baseUri)
        {
            _baseUri = baseUri;
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
                        var reply2 = await argument as Activity;
                        Activity reply = reply2.CreateReply();
                        string base64String;
                        string image64;
                        try
                        {
                            using (Image image = Image.FromFile(HttpContext.Current.Server.MapPath("~/bomb300.png")))
                            {
                                using (MemoryStream m = new MemoryStream())
                                {
                                    image.Save(m, image.RawFormat);
                                    byte[] imageBytes = m.ToArray();
            
                                    // Convert byte[] to Base64 String
                                    base64String = Convert.ToBase64String(imageBytes);
                                }
                            }
                            image64 = "data:image/png;base64," + base64String;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }

//            var reply2 = await argument as Activity;
//            Activity reply = reply2.CreateReply();
//            string base64String;
//            string image64;
//            Attachment pdf;
//            try
//            {
//                //This was written for images, so you can display something else here if you would like
//                Byte[] bytes = File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/bomb300.png"));
//                base64String = Convert.ToBase64String(bytes);
//                image64 = "data:image/png;base64," + base64String;
//                reply.Attachments = new List<Attachment>();
//                pdf = new Attachment(contentType: "image/png", content: image64);
//                reply.Attachments.Add(pdf);
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e);
//                throw;
//            }
//            await context.PostAsync(reply);
//
//
            var absolute = System.Web.VirtualPathUtility.ToAbsolute("~/bomb300.png");
            Uri resourceFullPath = new Uri(_baseUri, absolute);
            HeroCard heroCard = new HeroCard()
            {
                Images = new List<CardImage>
                {
                    new CardImage(
                        url: image64)
                },
                Tap = new CardAction()
                {
                    Value = resourceFullPath,
                    Type = "openUrl",
                }
            };

            reply.Attachments = new List<Attachment>
            {
                heroCard.ToAttachment()
            };
            await context.PostAsync(reply);
            context.Wait(MessageReceivedAsync);
        }
    }
}
