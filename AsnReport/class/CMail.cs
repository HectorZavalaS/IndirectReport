using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace smtLocations.Class
{
    public class CMail
    {
        //smtlocations!
        string From = ""; //de quien procede, puede ser un alias
        string To;  //a quien vamos a enviar el mail
        string m_Message;  //mensaje
        string Subject; //asunto
        List<string> Archivo = new List<string>(); //lista de archivos a enviar
        //string DE = "ASN_Report@siix.mx"; //nuestro usuario de smtp
        //string PASS = "ASN_Report1"; //nuestro password de smtp

        string DE = "siixsem.reports@siix-global.com"; //nuestro usuario de smtp
        string PASS = "S11x.r3p2024!"; //nuestro password de smtp

        //string DE = "javier.gallardo@siix-sem.com.mx"; //nuestro usuario de smtp
        //string PASS = "Zus67926"; //nuestro password de smtp

        System.Net.Mail.MailMessage Email;

        public string error = "";

        public string Message { get => m_Message; set => m_Message = value; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="FROM">Procedencia</param>
        /// <param name="Para">Mail al cual se enviara</param>
        /// <param name="Mensaje">Mensaje del mail</param>
        /// <param name="Asunto">Asunto del mail</param>
        /// <param name="ArchivoPedido_">Archivo a adjuntar, no es obligatorio</param>
        public CMail(string FROM, string Para, string Mensaje, string Asunto, List<string> ArchivoPedido_ = null)
        {
            From = FROM;
            To = Para;
            Message = Mensaje;
            Subject = Asunto;
            Archivo = ArchivoPedido_;
            //Email.
        }

        /// <summary>
        /// metodo que envia el mail
        /// </summary>
        /// <returns></returns>
        public bool enviaMail(ref String error)
        {
            bool result = false;
            
            //una validación básica
            if (To.Trim().Equals("") || Message.Trim().Equals("") || Subject.Trim().Equals(""))
            {
                error = "El mail, el asunto y el mensaje son obligatorios";
                return result;
            }

            //aqui comenzamos el proceso
            //comienza-------------------------------------------------------------------------
            try
            {
                //creamos un objeto tipo MailMessage
                //este objeto recibe el sujeto o persona que envia el mail,
                //la direccion de procedencia, el asunto y el mensaje
                Email = new System.Net.Mail.MailMessage(From, "antonio.hernandez@siix-global.com", Subject, Message);

                Email.To.Clear();

                foreach (var address in To.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    Email.To.Add(address);
                }
                //Email.CC.Add("nami.watanabe@SIIX-SEM.com.mx");
                //Email.CC.Add("cristobal.munoz@siix-sem.com.mx");
                //si viene archivo a adjuntar
                //realizamos un recorrido por todos los adjuntos enviados en la lista
                //la lista se llena con direcciones fisicas, por ejemplo: c:/pato.txt
                if (Archivo != null)
                {
                    //agregado de archivo
                    foreach (string archivo in Archivo)
                    {
                        //comprobamos si existe el archivo y lo agregamos a los adjuntos
                        if (System.IO.File.Exists(@archivo))
                            Email.Attachments.Add(new System.Net.Mail.Attachment(@archivo));

                    }
                }

                Email.IsBodyHtml = true; //definimos si el contenido sera html
                Email.From = new System.Net.Mail.MailAddress(From); //definimos la direccion de procedencia

                //aqui creamos un objeto tipo SmtpClient el cual recibe el servidor que utilizaremos como smtp
                //en este caso me colgare de gmail
                System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)(768 | 3072);
                System.Net.Mail.SmtpClient smtpMail = new System.Net.Mail.SmtpClient("smtp.office365.com");

                smtpMail.EnableSsl = true;
                smtpMail.UseDefaultCredentials = false; //le decimos que no utilice la credencial por defecto
                smtpMail.Host = "smtp.office365.com"; //agregamos el servidor smtp
                smtpMail.Port = 587; //le asignamos el puerto, en este caso gmail utiliza el 465
                smtpMail.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpMail.Credentials = new System.Net.NetworkCredential(DE, PASS); //agregamos nuestro usuario y pass de gmail

                //enviamos el mail
                smtpMail.Send(Email);

                //eliminamos el objeto
                smtpMail.Dispose();

                //regresamos true
                result = true;
            }
            catch (Exception ex)
            {
                //si ocurre un error regresamos false y el error
                error = "Ocurrio un error: " + ex.Message;
                result = false;
            }

            return result;
        }
    }
}