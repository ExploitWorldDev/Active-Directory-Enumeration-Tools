using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Principal;

class Program
{
    static void Main()
    {
        try
        {
            DirectoryEntry RootDSE = new DirectoryEntry("LDAP://RootDSE");
            string defaultNamingContext = RootDSE.Properties["defaultNamingContext"].Value.ToString();
            DirectoryEntry dommainpath = new DirectoryEntry("LDAP://" + defaultNamingContext);

            Console.WriteLine("Fetching GMSA Account...");
            List<DirectoryEntry> gmsaAccounts = GetGmsaAccounts(dommainpath);

            Console.WriteLine($"Found {gmsaAccounts.Count} gMSA Accounts:");
            foreach (DirectoryEntry gmsa in gmsaAccounts)
            {
                string name = gmsa.Properties["name"].Value.ToString();
                string dn = gmsa.Properties["distinguishedName"].Value.ToString();

                Console.WriteLine($"- {name} ({dn})");
                PrintPermission(gmsa);
                Console.WriteLine();
            }


            Console.WriteLine("Fatching Computer With LAPS Passwords...");
            List<DirectoryEntry> lapscomputers = GetLAPSComputers(dommainpath);
            Console.WriteLine($"Found {lapscomputers.Count} Laps Accounts:");
            foreach (DirectoryEntry lapscom in lapscomputers)
            {
                string name = lapscom.Properties["name"].Value.ToString();
                string dn = lapscom.Properties["distinguishedName"].Value.ToString();

                Console.WriteLine($"- {name} ({dn})");
                PrintPermission(lapscom);
                Console.WriteLine();
            }



        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR" + ex.Message);
        }

        Console.WriteLine("Done.");

    }



    static List<DirectoryEntry> GetGmsaAccounts(DirectoryEntry domainroot)
    {

        List<DirectoryEntry> gmsaList = new List<DirectoryEntry>();

        using (DirectorySearcher ds = new DirectorySearcher(domainroot))
        {
            ds.Filter = "(objectClass=msDS-GroupManagedServiceAccount)";
            ds.PropertiesToLoad.Add("name");
            ds.PropertiesToLoad.Add("distinguishedName");
            ds.PageSize = 1000;

            foreach (SearchResult sr in ds.FindAll())
            {
                DirectoryEntry de = sr.GetDirectoryEntry();
                gmsaList.Add(de);
            }
        }
        return gmsaList;

    }


    static List<DirectoryEntry> GetLAPSComputers(DirectoryEntry domainroot)
    {
        List<DirectoryEntry> lapsList = new List<DirectoryEntry>();
        using (DirectorySearcher ds = new DirectorySearcher(domainroot))
        {
            ds.Filter = "(&(objectClass=computer)(ms-Mcs-AdmPwd=*))";
            ds.PropertiesToLoad.Add("name");
            ds.PropertiesToLoad.Add("distinguishedName");
            ds.PropertiesToLoad.Add("ms-Mcs-AdmPwd");
            ds.PageSize = 1000;

            foreach (SearchResult sr in ds.FindAll())
            {
                DirectoryEntry de = sr.GetDirectoryEntry();
                lapsList.Add(de);
            }
        }
        return lapsList;
    }

    static void PrintPermission(DirectoryEntry entry)
    {
        try
        {

            ActiveDirectorySecurity security = entry.ObjectSecurity;

            AuthorizationRuleCollection acl = security.GetAccessRules(true, true, typeof(SecurityIdentifier));

            Console.WriteLine("Permissions:");

            foreach (ActiveDirectoryAccessRule rule in acl)
            {
                string identify = new SecurityIdentifier(rule.IdentityReference.Value).Translate(typeof(NTAccount)).Value;
                string accessType = rule.AccessControlType.ToString();
                string rights = rule.ActiveDirectoryRights.ToString();
                string inheritance = rule.InheritanceFlags.ToString();
                string ObejctType = rule.ObjectType.ToString();


                Console.WriteLine($"    Principle: {identify}");
                Console.WriteLine($"    Access: {accessType}");
                Console.WriteLine($"    Rights: {rights}");
                Console.WriteLine($"    Inheritance :{inheritance}");
                Console.WriteLine($"    objectType: {ObejctType}");

            }


        }
        catch (Exception ex)
        {
            Console.WriteLine("Could Not Reterive Permission:" + ex.Message);
        }

    }
}