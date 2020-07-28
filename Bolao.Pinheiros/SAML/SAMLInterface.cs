using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace Bolao.Pinheiros.SAML
{
    public class SAMLRequest
    {
        public string EncodeSamlAuthnRequest(string authnRequest)
        {
            var bytes = Encoding.UTF8.GetBytes(authnRequest);
            using (var output = new MemoryStream())
            {
                using (var zip = new DeflateStream(output, CompressionMode.Compress))
                {
                    zip.Write(bytes, 0, bytes.Length);
                }
                var base64 = Convert.ToBase64String(output.ToArray());
                return base64;
            }
        }

        public string GetSAMLRequest(string strACSUrl, string strIssuer)
        {
            using (StringWriter SWriter = new StringWriter())
            {
                var xWriterSettings = new XmlWriterSettings();
                xWriterSettings.OmitXmlDeclaration = true;

                using (var xWriter = XmlWriter.Create(SWriter, xWriterSettings))
                {
                    xWriter.WriteStartElement("samlp", "AuthnRequest", "urn:oasis:names:tc:SAML:2.0:protocol");
                    xWriter.WriteAttributeString("ID", "_" + System.Guid.NewGuid().ToString());
                    xWriter.WriteAttributeString("Version", "2.0");
                    xWriter.WriteAttributeString("IssueInstant", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"));
                    xWriter.WriteAttributeString("ProtocolBinding", "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST");
                    xWriter.WriteAttributeString("AssertionConsumerServiceURL", strACSUrl);

                    xWriter.WriteStartElement("saml", "Issuer", "urn:oasis:names:tc:SAML:2.0:assertion");
                    xWriter.WriteString(strIssuer);
                    xWriter.WriteEndElement();

                    xWriter.WriteStartElement("samlp", "NameIDPolicy", "urn:oasis:names:tc:SAML:2.0:protocol");
                    xWriter.WriteAttributeString("Format", "urn:oasis:names:tc:SAML:2.0:nameid-format:transient");
                    xWriter.WriteAttributeString("AllowCreate", "true");
                    xWriter.WriteEndElement();

                    xWriter.WriteStartElement("samlp", "RequestedAuthnContext", "urn:oasis:names:tc:SAML:2.0:protocol");
                    xWriter.WriteAttributeString("Comparison", "exact");

                    xWriter.WriteStartElement("saml", "AuthnContextClassRef", "urn:oasis:names:tc:SAML:2.0:assertion");
                    xWriter.WriteString("urn:oasis:names:tc:SAML:2.0:ac:classes:PasswordProtectedTransport");
                    xWriter.WriteEndElement();
                    xWriter.WriteEndElement();

                    xWriter.WriteEndElement();
                }

                return EncodeSamlAuthnRequest(SWriter.ToString());
            }
        }
    }

    public class SAMLResponse
    {
        public bool IsResponseValid(XmlDocument xDoc, byte[] certificado)
        {
            XmlNamespaceManager manager = new XmlNamespaceManager(xDoc.NameTable);
            manager.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);
            XmlNodeList nodeList = xDoc.SelectNodes("//ds:Signature", manager);

            var signedXml = new SignedXml(xDoc);
            signedXml.LoadXml((XmlElement)nodeList[0]);

            var cSigningCertificate = new X509Certificate2();

            cSigningCertificate.Import(certificado);

            return signedXml.CheckSignature(cSigningCertificate, true);
        }

        public string ParseSAMLNameID(XmlDocument xDoc)
        {
            XmlNamespaceManager xManager = new XmlNamespaceManager(xDoc.NameTable);
            xManager.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);
            xManager.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");
            xManager.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");

            XmlNode node = xDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:Subject/saml:NameID", xManager);
            return node.InnerText;
        }

        public string ParseSAMLAttribute(XmlDocument xDoc, string attributeName)
        {
            var xManager = new XmlNamespaceManager(xDoc.NameTable);
            xManager.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);
            xManager.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");
            xManager.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");

            var nodes = xDoc.SelectNodes("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute", xManager);
            foreach (XmlNode node in nodes)
            {
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    if (attribute.Value.ToUpperInvariant().Contains(attributeName.ToUpperInvariant()))
                    {
                        return node.InnerText;
                    }
                }
            }

            return string.Empty;
        }

        public XmlDocument ParseSAMLResponse(string strEncodedSAMLResponse)
        {
            var encencoder = new System.Text.ASCIIEncoding();
            var strCleanResponse = encencoder.GetString(Convert.FromBase64String(strEncodedSAMLResponse));

            var xDoc = new XmlDocument
            {
                PreserveWhitespace = true,
                XmlResolver = null
            };

            xDoc.LoadXml(strCleanResponse);

            return xDoc;
        }
    }
}