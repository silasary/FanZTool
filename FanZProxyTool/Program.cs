using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FanZProxyTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Directory.CreateDirectory("Export");
            Directory.CreateDirectory(Path.Combine("Export", "Images"));
            string stylecss = Path.Combine("Export", "style.css");
            if (!File.Exists(stylecss))
                File.Copy("style.css", stylecss);
            if (args.Length == 0)
            {
                args = Directory.GetFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "OCTGN", "Decks"), "*.o8d");
            }
            foreach (var deck in args)
            {
                PrintDeck(deck);
            }
        }

        private static void PrintDeck(string path)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\">")
                .Append("<html lang='en'>")
                .Append("<head>")
                .Append("   <link rel='stylesheet' type='text/css' href='style.css'>")
                .Append("</head>")
                .Append("<body>")
                .Append("<ul class='cards'>");
            var deck = XDocument.Load(path);
            var gameid = deck.Root.Attribute("game").Value;
            foreach (var card in deck.Root.Descendants("card"))
            {
                var qty = int.Parse(card.Attribute("qty").Value);
                var img = CopyFile(gameid, card.Attribute("id").Value, card.Value);
                for (int i = 0; i < qty; i++)
                {
                    html.Append($"<li class='fullcard'><img src='{img}' alt='{card.Value}'></li>");
                }
            }
            html.Append("</ul></body></html>");
            File.WriteAllText(Path.Combine("Export", Path.GetFileNameWithoutExtension(path) + ".html"), html.ToString());
        }

        private static string CopyFile(string gameid, string image_id, string name)
        {
            name = CleanInput(name);
            string searchPattern = $"{image_id}_{name}.*";
            var imagedir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "OCTGN", "ImageDatabase", gameid, "Sets");
            var search = Directory.GetFiles("Export", searchPattern, SearchOption.AllDirectories);
            string found = search.SingleOrDefault();
            if (found != null)
            {
                return $"Images/{Path.GetFileName(found)}";
            }
            search = Directory.GetFiles(imagedir, $"{image_id}.*", SearchOption.AllDirectories);
            try
            {
                found = search.SingleOrDefault();
            }
            catch (InvalidOperationException)
            {
                found = CollisionForm.Resolve(search, name);
            }
            if (found != null)
            {
                string finalname = $"{image_id}_{name}" + Path.GetExtension(found);
                string destFileName = Path.Combine("Export", "Images", finalname);
                using (var bitmap = Image.FromFile(found))
                {
                    if (bitmap.Width > bitmap.Height)
                    {
                        bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        bitmap.Save(destFileName);
                        return $"Images/{finalname}";
                    }
                }
                File.Copy(found, destFileName);
                return $"Images/{finalname}";
            }
            Console.WriteLine($"Couldn't find {image_id}!");
            return "";
        }

        static string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings.
            try
            {
                return Regex.Replace(strIn, @"[^\w\.@-]", "",
                                     RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            // If we timeout when replacing invalid characters, 
            // we should return Empty.
            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
            }
        }
    }
}
