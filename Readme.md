# How to Run 


1 - Open Properties
2 - Create a file named launchSettings.json
3 - Paste the code below 
``` 
{
  "profiles": {
    "NetFrame": {
      "commandName": "Project",
      "environmentVariables": {
        "ZOSMF_BASE_URL": "ZOSMF_BASE_URL:PORT",
        "ZOSMF_USERNAME": "ZOSMF_USERNAME",
        "ZOSMF_PASSWORD": "ZOSMF_PASSWORD"
      }
    }
  }
}
``` 
4 - Run the project

## You can review the ```ZosmfService.cs ``` to see how the system works.


## Documentation 
 
For the original documentation written by IBM you can visit the [ibm z/osmf programming guide](https://www.ibm.com/docs/en/SSLTBW_3.1.0/pdf/izua700_v3r1.pdf) 