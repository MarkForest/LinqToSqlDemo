using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;

namespace LinqToSqlDemo
{
    class Program
    {
        static string connectionString = @"Data Source=CR5-00\SQLEXPRESS;Initial Catalog=Library;Integrated Security=true;";
        
        static void Main(string[] args)
        {
            DataContext db = new DataContext(connectionString);
            //Get table Authors
            Table<Author> authors = db.GetTable<Author>();

            //Filter
            Console.WriteLine("\nFilter");
            var query = (from a in authors
                         where a.FirstName == "big"
                         orderby a.LastName
                         select a).ToList();
            foreach (var author in query)
            {
                Console.WriteLine($"{author.id}\t{author.FirstName}\t{author.LastName}");
            }

            //or
            //var query2 = authors.Where(a => a.FirstName == "big").OrderBy(a => a.LastName);

            Console.WriteLine("\nChanges");
            foreach (var author in authors)
            {
                Console.WriteLine($"{author.id}\t{author.FirstName}\t{author.LastName}");
            }

            var authorF = from a in authors where a.id == 5 select a;
            Author authorOne = authorF.First();
            Console.WriteLine(authorOne);
            authorOne.FirstName = "Cristiano";
            authorOne.LastName = "Ronaldo";
            db.SubmitChanges();
         
            foreach (var author in authors)
            {
                Console.WriteLine($"{author.id}\t{author.FirstName}\t{author.LastName}");
            }

            Console.WriteLine("\nAdded");
            Author newAuthor = new Author {FirstName = "Biba", LastName="Bobavich" };
            authors.InsertOnSubmit(newAuthor);
            db.SubmitChanges();

            Console.WriteLine("id added row = "+newAuthor.id);
            foreach (var author in authors)
            {
                Console.WriteLine($"{author.id}\t{author.FirstName}\t{author.LastName}");
            }

            Console.WriteLine("\nDeleted");
            Author AuthorD = authors.OrderByDescending(a => a.id).First();
            authors.DeleteOnSubmit(AuthorD);
            db.SubmitChanges();

            Console.WriteLine("\nExecute Command and Execute Query");

            db.ExecuteCommand("delete from authrs where id={0}", 5);
            var authors2 = db.ExecuteQuery<Author>("select * from authors where id={0}", 6);

            Console.WriteLine("\nPagination");
            int pageNumber = 0; //current page
            int pageSize = 2; //count element on page
            int count = 0; //count all rows

            count = authors.Count();
            if (count > pageNumber * pageSize)
            {
                var data = authors.Skip(pageNumber * pageSize).Take(pageSize);
                foreach (var author in data)
                {
                    Console.WriteLine($"{author.id}\t{author.FirstName}\t{author.LastName}");
                }
            }

            Console.Read();
        }
    }
}
