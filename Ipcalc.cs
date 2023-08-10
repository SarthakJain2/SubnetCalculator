using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using NetTools;

namespace IPCalc
{
    public class Program
    {
        static string filePath = "ipcalc.json";

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintHelp();
                return;
            }

            var vnets = LoadVnets();
            switch (args[0].ToLower())
            {
                case "register":
                    var vnetName = args[1];
                    var addressSpace = args[2];
                    vnets.Add(new VNet(vnetName, addressSpace));
                    SaveVnets(vnets);
                    break;
                case "verify":
                    var vnetNameVerify = args[1];
                    var vnet = vnets.FirstOrDefault(v => v.Name == vnetNameVerify);
                    if (vnet == null)
                    {
                        Console.WriteLine($"No VNet with name {vnetNameVerify} found.");
                        return;
                    }
                    var rangeToVerify = args[2];
                    var result = vnet.VerifyRange(IPNetwork.Parse(rangeToVerify));
                    Console.WriteLine($"Range {rangeToVerify} is {(result ? "" : "not ")}available.");
                    break;
                case "allocate":
                    var subnetName = args[1];
                    var vnetNameAllocate = args[2];
                    var vnetAllocate = vnets.FirstOrDefault(v => v.Name == vnetNameAllocate);
                    if (vnetAllocate == null)
                    {
                        Console.WriteLine($"No VNet with name {vnetNameAllocate} found.");
                        return;
                    }
                    var cidrOrRange = args[3];
                    var subnet = vnetAllocate.AllocateSubnet(subnetName, IPNetwork.Parse(cidrOrRange));
                    if (subnet != null)
                    {
                        Console.WriteLine($"Subnet {subnet.Name} with IP range {subnet.Range} allocated.");
                        SaveVnets(vnets);
                    }
                    else
                    {
                        Console.WriteLine("Allocation failed. No available range.");
                    }
                    break;
                default:
                    PrintHelp();
                    break;
            }
        }

        static void PrintHelp()
        {
            Console.WriteLine("Usage: IPCalc <command> <arguments>");
            Console.WriteLine("Commands: register, verify, allocate");
        }

        static List<VNet> LoadVnets()
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<VNet>>(json);
            }
            return new List<VNet>();
        }

        static void SaveVnets(List<VNet> vnets)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            };

            var json = JsonConvert.SerializeObject(vnets, settings);
            File.WriteAllText(filePath, json);
        }
    }

    public class VNet
    {
        public string Name { get; set; }
        public string AddressSpace { get; set; }
        public List<Subnet> Allocated { get; set; }

        public VNet(string name, string addressSpace)
        {
            Name = name;
            AddressSpace = addressSpace;
            Allocated = new List<Subnet>();
        }

public bool VerifyRange(IPNetwork range)
{
    var addressSpaceNetwork = IPNetwork.Parse(AddressSpace);
    if (!addressSpaceNetwork.Contains(range.Network) || !addressSpaceNetwork.Contains(range.Broadcast))
    {
        return false;
    }

    return !Allocated.Any(s => s.Range.Contains(range.Network) || s.Range.Contains(range.Broadcast));
}

public Subnet AllocateSubnet(string name, IPNetwork range)
{
    if (VerifyRange(range))
    {
        var subnet = new Subnet(name, range);
        Allocated.Add(subnet);
        return subnet;
    }
    return null;
}

    }

    public class Subnet
    {
        public string Name { get; set; }
        public int Cidr { get; set; }
        public IPNetwork Range { get; set; }
        public List<string> Addresses { get; set; }

        public Subnet(string name, IPNetwork range)
        {
            Name = name;
            Range = range;
            Cidr = range.Cidr;
            Addresses = new List<string>
            {
                Range.Network.ToString(),
                Range.Broadcast.ToString()
            };
        }
    }
}
