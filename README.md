# Active-Directory-Enumeration-Tools
---------------------------------------------------------------------------------------------------------------------------------------------------------

# Generic Rights Checker

## Prerequisites

- .NET SDK (or runtime) installed, compatible with the version 

A tool that enumerates the **generic rights** (e.g., `GenericAll`, `GenericWrite`, `GenericRead`) of the current user on the system in Active Directory Domain.

## Features

- Shows the **generic access rights** assigned to the current user context.
- Can list rights such as:
  - `GenericAll`
  - `GenericWrite`
  - `GenericRead`
  - `GenericExecute` (if applicable)


## Usage

Run the executable from a terminal:

```
C:\Users\Test> GenericRightsChecker.exe
                    Enumerating all AD objects for GenericAll/GenericWrite/GenericRead permissions...
                    Object: hello (DC=hello,DC=local)
                    ObjectClass: top, domain, domainDNS
                    Matching Permissions:
                      Principal: NT AUTHORITY\ENTERPRISE DOMAIN CONTROLLERS
                       Access Type: Allow
                       Rights: GenericRead
                       Inheritance: None
                       ObjectType: 00000000-0000-0000-0000-000000000000
                    Done.
```


# Nested Group Finder

A tool that discovers **nested group memberships**, i.e., groups that are members of other groups (member‑of‑member relationships) in Active Directory.

## Features

- Lists all groups that are members of a given group (direct members).
- Recursively expands nested groups, showing the full hierarchy:
  - Group A → Group B → Group C

## Usage

Run the executable from a terminal:

```
C:\Users\Test> NestedGroupFinder.exe
                    Groups with nested group Hierarchy
                     -Administrators
                         -Domain Admins
                         -Enterprise Admins
                     -Users
                         -Domain Users
                     -Guests
                         -Domain Guests
                     -Print Operators
                     -Backup Operators
```



# AD User Enumerator

A tool that retrieves all user accounts from Active Directory and displays key attributes for each user.

## Attributes shown

For each user, the tool outputs:

- `sAMAccountName` – the user’s logon name (pre‑Windows 2000 style).
- `displayName` – the user’s display name (e.g., “John Doe”).
- `distinguishedName` – the full LDAP path of the user object.
- `description` – the user’s description field (if set).


## Usage

Run the executable:

```
C:\Users\Test> ADUserEnumerator.exe
                UserName                      Display Name                  Distinguished Name                                                                                  Description                                       
                ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                Administrator                 N/A                           CN=Administrator,CN=Users,DC=hello,DC=local                                                           Built-in account for administering the computer/domain
                Guest                         N/A                           CN=Guest,CN=Users,DC=hello,DC=local                                                                   Built-in account for guest access to the computer/domain
                DefaultAccount                N/A                           CN=DefaultAccount,CN=Users,DC=hello,DC=local                                                          A user account managed by the system.  

```


# Subnet and Port Scanner

A C# console application that scans an entire subnet and checks a minimal set of common ports on each live host.

## Features

- Scans all IPs in a given subnet (e.g., `192.168.8.0/24`).
- Tests a predefined set of ports on active hosts:
  `20, 21, 22, 23, 25, 53, 80, 110, 143, 443, 445, 993, 995, 3306, 3389`.

## Usage

Run the executable with a subnet prefix (IPv4 only):
```
C:\Users\Test> SubnetPortEnum.exe 192.168.0
                    Scanning 192.168.0.1...
                      Port 22 is open on 192.168.0.1
                      Port 80 is open on 192.168.0.1
                      Port 443 is open on 192.168.0.1
                    Scanning 192.168.0.2...
                    Scanning 192.168.0.3...
                    Scanning 192.168.0.4...
                    Scanning 192.168.0.5...
                      Port 445 is open on 192.168.0.5
                      Port 3389 is open on 192.168.0.5

```

# LAPS and gMSA Enumerator

A tool that discovers and lists:
- Computers with **LAPS** (Local Administrator Password Solution) enabled.
- **gMSA** (Group Managed Service Account) computer accounts in Active Directory.


## Usage

Run the executable:

```
C:\Users\Test> LapsGmsaChecker.exe
                  Fetching GMSA Account...
                  Found 0 gMSA Accounts:
                  Fatching Computer With LAPS Passwords...
                  Found 0 Laps Accounts:
                  Done.
```


# AD Group Enumerator

A tool that enumerates Active Directory groups and shows:
- The group’s **display name**.
- The **list of members** (users, computers, or other groups).
- The **distinguishedName** (full LDAP path).
- The **description** (if set).

## What it shows

For each group, the tool outputs:

- `groupName` – the group’s name (e.g., `Domain Admins`).
- `displayName` – the group’s display name (if different from `groupName`).
- `members` – a list of member objects:
  - Type: `User`, `Computer`, or `Group`.
  - Name: the object’s name (e.g., `DOMAIN\alice`).
- `distinguishedName` – the full LDAP path (e.g., `CN=Domain Admins,CN=Users,DC=domain,DC=com`).
- `description` – the group’s description field (or `null` if not set).


## Usage

Run the executable:

```
C:\Users\Test> ADGroupEnumerator.exe
                Group Name                    Distinguished Name                                                    Description                   
                ----------------------------------------------------------------------------------------------------------------------------------
                Administrators                CN=Administrators,CN=Builtin,DC=hello,DC=local                        Administrators have complete and unrestricted access to the computer/domain
                 UserName                      DistinguishedName                                                    Description
                ----------------------------------------------------------------------------------------------------
                   tom                         
                   jerry                         
                   oggy                       
                   Domain Admins
                ----------------------------------------------------------------------------------------------------------------------------------
                DNSAdmin                       CN=DNSAdmin,DC=hello,DC=local                                        N/A                           
                 UserName                      DistinguishedName                                                    Description
              ----------------------------------------------------------------------------------------------------
                 jack
```



# AD AddSelf / WriteProperty Checker

A tool that scans Active Directory to find users that have **“Add self”** (self‑membership) or **“WriteProperty”**‑type permissions on other objects (users, groups, OUs, etc.).

## What it checks

- Looks for users that can:
  - Add themselves to a group (**“Add self” / self‑membership right**).
- Reports which user has these rights and on which target object (group, user, OU, or domain).

## Usage

Run the executable:

```
C:\Users\Test> ADAddSelfWriteChecker.exe
                Default Naming Context: DC=hello,DC=local
                Enumerating AddSelf ACL for Users:


```



# AD ACL Checker – Add Member & Force Change Password

A tool that scans Active Directory to find users or groups that have **“Add member”** or **“Force change password”** permissions on other objects (users, groups, OUs, or the domain).


## Features

- Scans the whole domain or a specified OU for delegated permissions.
- Detects:
  - “Add member” rights on groups and containers.
  - “Force change password” (reset password) rights on user objects.
- Outputs a simple list of risky ACLs for security audits and hardening.


## Usage

Run the executable:

```
C:\Users\Test> ADACLAddMemberForcePwd.exe
                Domain base: DC=hello,DC=local
                Checking Users for AddMember and ForceChangePassword ACLs...


```
