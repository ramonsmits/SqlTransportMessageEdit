using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Dapper;

namespace SqlMessageEdit
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: sqlmessageedit.exe {connectionstring key} {queue table name} ({message id})");
            }

            var cs = System.Configuration.ConfigurationManager.ConnectionStrings[args[0]].ConnectionString;
            var name = args[1];

            using (var con = new SqlConnection(cs))
            {
                con.Open();

                Guid id;

                if (args.Length < 3)
                {
                    var list = con.Query<Guid>("select Id from " + name).OrderBy(x => x).ToList();

                    for (int i = 0; i < list.Count; i++)
                    {
                        Console.WriteLine("{0,4}: {1}", i, list[i]);
                    }

                    Console.WriteLine("Enter message NR or ID to edit:");

                    string line;
                    line = Console.ReadLine();

                    if (!Guid.TryParse(line, out id))
                    {
                        var nr = int.Parse(line);
                        id = list[nr];
                    }
                }
                else
                {
                    id = Guid.Parse(args[2]);
                }


                var body = con.Query<byte[]>("select Body from " + name + " where id=@id", new { id }).First();

                var encoding = Encoding.UTF8;
                var text = encoding.GetString(body);

                Console.WriteLine("Message body:");
                Console.WriteLine(text);

                Console.WriteLine("Edit message? Y/N [N]");
                var edit = Console.ReadLine();

                if (edit.ToUpperInvariant() == "Y")
                {
                    string ext;

                    if (text.StartsWith("<"))
                    {
                        ext = "xml";
                    }
                    else if (text.StartsWith("?{") | text.StartsWith("{"))
                    {
                        ext = "json";
                    }
                    else
                    {
                        ext = "bin";
                    }

                    var fn = Path.Combine(Path.GetTempPath(), "message-" + id + "." + ext);

                    File.WriteAllBytes(fn, body);

                    Console.WriteLine("Message body written to: {0}", fn);

                    Console.WriteLine("Enter 'UPDATE'  when finished editing and wanting to update body.");

                    var update = Console.ReadLine();

                    if (update == "UPDATE")
                    {
                        body = File.ReadAllBytes(fn);

                        con.Execute("update " + name + " set body=@body where id=@id", new { id, body });
                    }

                    File.Delete(fn);
                }
            }
        }
    }
}
