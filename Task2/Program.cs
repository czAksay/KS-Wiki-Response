using System;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace Task2
{
    class Program
    {
        static String GetResponse(String title)
        {
            if (String.IsNullOrEmpty(title))
                throw new EmptyTitleStringException();
            String url = "https://en.wikipedia.org/w/api.php?format=json&action=query&prop=extracts&exintro&explaintext&redirects=1&titles=";
            WebRequest request = WebRequest.Create(url + title);
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string receive = sr.ReadToEnd();
            sr.Close();
            return ExtractArticle(receive);
        }

        static String ExtractArticle(String text)
        {
            Regex regex = new Regex("\"extract\":\"(.*)\"}}}}");
            //Match match = regex.Match(text);
            if (regex.Matches(text).Count == 0)
                throw new NoSuchTitleException();
            text = regex.Match(text).Groups[1].Value;
            text = text.Replace(@"\r\n", "\n");
            text = text.Replace(@"\n", "\n");
            text = text.Replace(@"\""", "\"");
            return text;
        }

        static void Main(string[] args)
        {
            bool finished;
            do
            {
                Console.WriteLine("Enter title to search:");
                String title = Console.ReadLine();
                try
                {
                    String response = GetResponse(title);
                    Console.WriteLine("Wikipedia says:\n" + response);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                char consoleLetter;
                do
                {
                    Console.WriteLine("Search again? y/n");
                    consoleLetter = Console.ReadKey().KeyChar;
                    Console.WriteLine();
                }
                while (consoleLetter != 'n' && consoleLetter != 'y');
                finished = consoleLetter == 'n';
            }
            while (!finished);
        }
    }

    class NoSuchTitleException : Exception
    {
        public NoSuchTitleException() : base("Can't find such title!") { }
    }
    class EmptyTitleStringException : Exception
    {
        public EmptyTitleStringException() : base("Title can't be empty!") { }
    }
}
