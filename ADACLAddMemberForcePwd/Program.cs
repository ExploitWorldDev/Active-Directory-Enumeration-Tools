using System;
using System.DirectoryServices;
using System.Security.AccessControl;
using System.Security.Principal;

class Program
{
    static void Main()
    {
        try
        {
            DirectoryEntry rootDse = new DirectoryEntry("LDAP://RootDSE");
            string domainBase = rootDse.Properties["defaultNamingContext"].Value.ToString();
            Console.WriteLine($"Domain base: {domainBase}\n");

            var objectFilters = new (string Filter, string Type)[]
            {
                ("(&(objectCategory=user)(objectClass=user))", "User"),
                ("(&(objectCategory=group)(objectClass=group))", "Group"),
                ("(&(objectCategory=computer)(objectClass=computer))", "Computer")
            };

            // GUIDs for the extended rights to check
            Guid addMemberGuid = new Guid("bf967aba-0de6-11d0-a285-00aa003049e2");
            Guid forceChangePwdGuid = new Guid("00299570-246d-11d0-a768-00aa006e0529");

            foreach (var (filter, type) in objectFilters)
            {
                Console.WriteLine($"Checking {type}s for AddMember and ForceChangePassword ACLs...\n");

                DirectoryEntry domainEntry = new DirectoryEntry($"LDAP://{domainBase}");
                DirectorySearcher searcher = new DirectorySearcher(domainEntry)
                {
                    Filter = filter
                };
                searcher.PropertiesToLoad.Add("distinguishedName");

                foreach (SearchResult sr in searcher.FindAll())
                {
                    string dn = sr.Properties["distinguishedName"][0].ToString();
                    DirectoryEntry objEntry = new DirectoryEntry($"LDAP://{dn}");
                    ActiveDirectorySecurity security = objEntry.ObjectSecurity;

                    var accessRules = security.GetAccessRules(true, true, typeof(SecurityIdentifier));

                    foreach (ActiveDirectoryAccessRule rule in accessRules)
                    {
                        if (rule.AccessControlType == AccessControlType.Allow &&
                           (rule.ActiveDirectoryRights & ActiveDirectoryRights.WriteProperty) == ActiveDirectoryRights.WriteProperty)
                        {
                            if (rule.ObjectType == addMemberGuid || rule.ObjectType == forceChangePwdGuid)
                            {
                                var trusteeName = rule.IdentityReference.Translate(typeof(NTAccount));
                                string rightName = (rule.ObjectType == addMemberGuid) ? "AddMember" : "ForceChangePassword";

                                Console.WriteLine($"{type}: {dn}");
                                Console.WriteLine($"  Trustee: {trusteeName.Value}");
                                Console.WriteLine($"  Right: {rightName}\n");
                            }
                        }
                    }
                }

                Console.WriteLine(new string('-', 60) + "\n");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
