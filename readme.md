# IP Address Calculator

## Description
This is a command-line interface (CLI) application for managing Virtual Networks (VNets) and Subnets. It is useful for keeping track of IP address spaces in VNets and helps in efficiently allocating new subnets without IP overlap. The information about the current VNets and Subnets is stored in a JSON file.

## Functions
The program provides the following functionalities:

1. **Register**: This function allows you to register a new VNet with its address space.
2. **Verify**: This function verifies whether a particular IP range can be allocated in a VNet. It checks if the given range is within the VNet's address space and not overlapping with any allocated Subnets.
3. **Allocate**: This function allocates a new Subnet in a given VNet. If the allocation is successful, it will update the JSON file with the new Subnet information.

## How to run
1. Clone or download the repository.
2. Navigate to the project directory in your terminal.
3. Run the following command to build the project: `dotnet build`
4. Execute one of the following commands:

   - To register a new VNet:
     ```
     dotnet run register <VNet-Name> <VNet-AddressSpace>
     ```
     Example: 
     ```
     dotnet run register myVnet 192.168.0.0/16
     ```
     
   - To verify a IP range in a VNet:
     ```
     dotnet run verify <VNet-Name> <IP-Range>
     ```
     Example: 
     ```
     dotnet run verify myVnet 192.168.0.0/24
     ```
     
   - To allocate a new Subnet in a VNet:
     ```
     dotnet run allocate <Subnet-Name> <VNet-Name> <IP-Range>
     ```
     Example: 
     ```
     dotnet run allocate mySubnet myVnet 192.168.0.0/24
     ```

Please replace `<VNet-Name>`, `<VNet-AddressSpace>`, `<Subnet-Name>`, and `<IP-Range>` with your actual values.

> Note: The IP range should be given in CIDR notation (e.g., 192.168.0.0/24).
