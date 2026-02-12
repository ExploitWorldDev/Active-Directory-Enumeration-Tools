using System;
using System.Diagnostics.Eventing.Reader;
using System.DirectoryServices;

class Program
{
    static void Main()
    {
        try
        {
            DirectoryEntry rootDSE = new DirectoryEntry("LDAP://RootDSE");
            string defaultNamingContext = rootDSE.Properties["defaultNamingContext"].Value.ToString();

            DirectoryEntry domainRoot = new DirectoryEntry("LDAP://" + defaultNamingContext);

            DirectorySearcher groupSearcher = new DirectorySearcher(domainRoot)
            {
                Filter = "(objectClass=group)"
            };

            groupSearcher.PropertiesToLoad.Add("name");
            groupSearcher.PropertiesToLoad.Add("distinguishedName");
            groupSearcher.PropertiesToLoad.Add("description");
            groupSearcher.PropertiesToLoad.Add("member");



            SearchResultCollection groups = groupSearcher.FindAll();


            Console.WriteLine(
                    "Group Name".PadRight(30) +
                    "Distinguished Name".PadRight(70) +
                    "Description".PadRight(30)
                );

            Console.WriteLine(new string('-', 130));

            foreach (SearchResult group in groups)
            {
                string GroupName = group.Properties["name"].Count > 0 ? group.Properties["name"][0].ToString() : "N/A";
                string DistinguishedName = group.Properties["distinguishedName"].Count > 0 ? group.Properties["distinguishedName"][0].ToString() : "N/A";
                string Description = group.Properties["description"].Count > 0 ? group.Properties["description"][0].ToString() : "N/A";
                string Member = group.Properties["member"].Count > 0 ? group.Properties["member"][0].ToString() : "N/A";

                Console.WriteLine($"{GroupName.PadRight(30)}{DistinguishedName.PadRight(70)}{Description.PadRight(30)}");

                if (group.Properties["member"].Count > 0)
                {
                    Console.WriteLine(" UserName".PadRight(30) + " DistinguishedName".PadRight(70) + "Description");
                    Console.WriteLine(new string('-', 100));

                    foreach (object MemberDNobj in group.Properties["member"])
                    {
                        string memberDN = MemberDNobj.ToString();
                        try
                        {
                            DirectoryEntry memberentry = new DirectoryEntry("LDAP://" + memberDN);

                            string memberName = memberentry.Properties["sAMAccountName"].Count > 0 ? memberentry.Properties["sAMAccountName"][0].ToString() : "N/A";
                            Console.WriteLine("   " + memberName.PadRight(30));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error Reading Members" + ex.Message);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No Members Found");
                }

                Console.WriteLine(new string('-', 130));
            }
        }
        catch (Exception ex) { Console.WriteLine("Error" + ex.Message); }
    }
}