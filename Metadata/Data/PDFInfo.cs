using System;
using System.Collections.Generic;
using PdfSharp.Pdf;

namespace Metadata.Data
{
    public class PDFInfo : BaseFileInfo
    {
        public string FileTitle { get; set; }
        public string FileAuthor { get; set; }
        public string FileCreator { get; set; }
        public string FileCreationDate { get; set; }
        public string FileModificationDate { get; set; }
        public string FileKeywords { get; set; }
        public string FileSubject { get; set; }
        public string FileReference { get; set; }
        public string FileProducer { get; set; }

        public PDFInfo() { }

        // dynamic 
        public PDFInfo GetAllPDFInfo(PdfDocument document, string name, string fullpath)
        {
            return new PDFInfo
            {
                FileTitle = GetTitle(document),
                FileAuthor = GetAuthor(document),
                FileCreator = GetCreator(document),
                FileCreationDate = GetCreationDate(document),
                FileModificationDate = GetModificationDate(document),
                FileKeywords = GetKeywords(document),
                FileSubject = GetSubject(document),
                FileReference = GetReference(document),
                FileProducer = GetProducer(document),
                FileName = name,
                FilePath = fullpath,
                FileSize = BytesToString(document.FileSize),
                FileType = "PDF"
            };
        }

        public override Dictionary<string, string> GetInformation()
        {
            return new Dictionary<string, string>
            {
                {"Author", FileAuthor },
                {"Title", FileTitle },
                {"Subject", FileSubject },
                {"Keywords", FileKeywords },
                {"Created on", FileCreationDate },
                {"Modified on", FileModificationDate },
                {"Creator", FileCreator },
                {"Producer", FileProducer },
                {"Reference" , FileReference},
                {"Type", FileType }
            };
        }

        private string GetTitle(PdfDocument document)
        {
            try
            {
                return document.Info.Title.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        private string GetAuthor(PdfDocument document)
        {
            try
            {
                return document.Info.Author.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        private string GetCreator(PdfDocument document)
        {
            try
            {
                return document.Info.Creator.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }


        private string GetCreationDate(PdfDocument document)
        {
            try
            {
                return document.Info.CreationDate.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        private string GetModificationDate(PdfDocument document)
        {
            try
            {
                return document.Info.ModificationDate.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        private string GetKeywords(PdfDocument document)
        {
            try
            {
                return document.Info.Keywords.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        private string GetProducer(PdfDocument document)
        {
            try
            {
                return document.Info.Producer.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        private string GetReference(PdfDocument document)
        {
            try
            {
                return document.Info.Reference.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        private string GetSubject(PdfDocument document)
        {
            try
            {
                return document.Info.Subject.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
