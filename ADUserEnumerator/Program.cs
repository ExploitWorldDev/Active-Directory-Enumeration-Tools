using System;
using System.DirectoryServices;



class Program
{
    static void Main()
    {
        try

        {
            DirectoryEntry entry = new DirectoryEntry("LDAP://RootDSE");
            string defaultNamingContext = entry.Properties["defaultNamingContext"].Value.ToString();

            DirectoryEntry rootSearch = new DirectoryEntry("LDAP://" + defaultNamingContext);

            DirectorySearcher searcher = new DirectorySearcher(rootSearch);
            searcher.Filter = "(&(objectCategory=person)(objectClass=user))";

            searcher.PropertiesToLoad.Add("sAMAccountName");
            searcher.PropertiesToLoad.Add("displayName");
            searcher.PropertiesToLoad.Add("distinguishedName");
            searcher.PropertiesToLoad.Add("description");

            SearchResultCollection results = searcher.FindAll();


            int col1Width = 30;
            int col2Width = 30;
            int col3Width = 100;
            int col4Width = 50;
            int tableWidth = col1Width + col2Width + col3Width + col4Width;

            Console.WriteLine(
                "UserName".PadRight(col1Width) +
                "Display Name".PadRight(col2Width) +
                "Distinguished Name".PadRight(col3Width) +
                "Description".PadRight(col4Width)
                );

            Console.WriteLine(new string('-', tableWidth));

            foreach (SearchResult result in results)
            {
                string username = result.Properties["sAMAccountName"].Count > 0 ? result.Properties["sAMAccountName"][0].ToString() : "N/A";
                string displayname = result.Properties["displayName"].Count > 0 ? result.Properties["displayName"][0].ToString() : "N/A";
                string distinguishedName = result.Properties["distinguishedName"].Count > 0 ? result.Properties["distinguishedName"][0].ToString() : "N/A";
                string description = result.Properties["description"].Count > 0 ? result.Properties["description"][0].ToString() : "N/A";

                Console.WriteLine(
                    username.PadRight(col1Width) +
                    displayname.PadRight(col2Width) +
                    distinguishedName.PadRight(col3Width) +
                    description.PadRight(col4Width)
                );

            }
            Console.WriteLine(new string('-', tableWidth));
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error" + ex.Message);
        }
    }
}