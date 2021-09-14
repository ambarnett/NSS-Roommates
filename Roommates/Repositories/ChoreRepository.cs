using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Roommates.Models;

namespace Roommates.Repositories
{
    public class ChoreRepository : BaseRepository
    {
        public ChoreRepository(string connectionString) : base(connectionString) { }

        public List<Chore> GetAll()
        {
            /// <summary>
            /// We must "use" the database connection.
            /// Because a database is a shared resource (other application my be using it too) we must
            /// be careful about how we interact with it. Specifically, we Open() connection when we need to
            /// interact with the database and we Close() them when we're finished.
            /// In C#, a "using" block ensures we correctly disconnect from a resource even if there is an error.
            /// For database connections, this means the connection will be properly closed.
            /// </summary>
            using (SqlConnection conn = Connection)
            {
                conn.Open(); //NOTE, we must Open() the connection, the "using" block doesn't do that for us.

                using (SqlCommand cmd = conn.CreateCommand()) // we must "use" commands too.
                {
                    cmd.CommandText = "SELECT Id, Name FROM Chore"; // Here we setup the command with the SQL we want to execute before we execute it.

                    SqlDataReader reader = cmd.ExecuteReader(); // Execute the SQL in the database and get a "reader" taht will give us access to the data.

                    List<Chore> chores = new List<Chore>(); //A list to hold the rooms we retrieve from the database.

                    while (reader.Read()) //Read() will return true if there's more data to read
                    {
                        // The "ordinal" in the numeric position of the column in the query results.
                        // for our query, "Id" has an ordinal value of 0 and "Name" is 1.
                        int idColumnPosition = reader.GetOrdinal("Id");

                        // we use the reader's GetXXX methods to get the value for a particular ordinal.
                        int idValue = reader.GetInt32(idColumnPosition);

                        int nameColumnPosition = reader.GetOrdinal("Name");
                        string nameValue = reader.GetString(nameColumnPosition);

                        Chore chore = new Chore // Now create a new chore object using the data from the database.
                        {
                            Id = idValue,
                            Name = nameValue
                        };

                        chores.Add(chore); // ...and add that chore object to our list.
                    }

                    // We should Close() the reader. Unfortunately, a "using" block won't work here.
                    reader.Close();

                    // Return the list of chores who whomever called this method.
                    return chores;
                }
            }
        }
    }
}
