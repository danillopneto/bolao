using Bolao.Pinheiros.SAML;
using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace Bolao.Pinheiros.Controllers
{
    public abstract class SamlController : Controller
    {
        private const string ENTITY_ID = "https://bolao.azurewebsites.net/";

        private const string KEY_LOGIN_SAML = "SAMLLogin";

        private const string KEY_RESPONSE_SAML = "SAMLResponse";

        private const string KEY_USER_SAML = "SAMLUser";

        private const string LOGIN_URL = "https://login.microsoftonline.com/ae9c23da-241e-43a1-ba46-4b56889877b9/saml2";

        private const string REPLY_URL = "https://bolao.azurewebsites.net/";

        private const string URL_CERTIFICATE = "https://login.microsoftonline.com/ae9c23da-241e-43a1-ba46-4b56889877b9/federationmetadata/2007-06/federationmetadata.xml?appid=a69be944-19a1-461e-afdf-62e314e21e50";

        private const string USER_ATTRIBUTE = "mail";

        private bool IsLoggedIn
        {
            get
            {
                return Session[KEY_LOGIN_SAML] != null
                        && Session[KEY_LOGIN_SAML].ToString() == true.ToString();
            }

            set
            {
                if (Session[KEY_LOGIN_SAML] != null)
                {
                    Session[KEY_LOGIN_SAML] = value;
                }
                else
                {
                    Session.Add(KEY_LOGIN_SAML, true);
                }
            }
        }

        private string SamlUser
        {
            get
            {
                return Session[KEY_USER_SAML] != null
                        ? Session[KEY_USER_SAML].ToString()
                        : string.Empty;
            }

            set
            {
                Session[KEY_USER_SAML] = value;
            }
        }

        public void CheckOrDoLogin()
        {
            var respostaSaml = Request.Form[KEY_RESPONSE_SAML];
            if (respostaSaml != null)
            {
                IsLoggedIn = true;

                var samlResponse = new SAMLResponse();
                var xDoc = samlResponse.ParseSAMLResponse(respostaSaml);
                var certificado = GetCertificateData(URL_CERTIFICATE);

                if (samlResponse.IsResponseValid(xDoc, certificado))
                {
                    SamlUser = samlResponse.ParseSAMLAttribute(xDoc, USER_ATTRIBUTE);
                }
                else
                {
                    throw new InvalidOperationException("Resposta SAML do IDP (Provedor de identidade não foi aceita.");
                }
            }
            else if (!IsLoggedIn)
            {
                var request = new SAMLRequest();
                var url = string.Concat(
                                        LOGIN_URL,
                                        "?SAMLRequest=",
                                        HttpUtility.UrlEncode(request.GetSAMLRequest(REPLY_URL, ENTITY_ID)));
                Response.Redirect(url);
            }
        }

        public bool IsSamlResponse()
        {
            return Request.Form[KEY_RESPONSE_SAML] != null;
        }

        private byte[] GetCertificateData(string urlCertificado)
        {
            var reader = new XmlTextReader(urlCertificado);
            var certificado = string.Empty;
            if (reader != null)
            {
                if (reader.ReadToDescendant("X509Certificate"))
                {
                    reader.Read();
                    certificado = reader.Value;
                }
            }

            return Encoding.ASCII.GetBytes(certificado);
        }
    }
}