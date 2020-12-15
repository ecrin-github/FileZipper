# FileZipper
Zips all or  selected MDR folders

This small program takes the files in one or more folders of XML or JSON folders and creates zip files out of them. 
This is largely to make it easier to move the data around, e.g. in the context of taking backups.

### Parameters
The system is a console app, (to more easily support being scheduled) and can take takes the following parameters:<br/>
**-s** followed by a comma separated list of integer ids, each representing a data source, and therefore folder, within the system.<br/>
**-A**: as a flag. If present, runs through all the sources / folders, and so allows the entire zipping operation to be done at once.<br/>
**-Z*: zips the json files created by the -F parameter into a series of zip files, with up to 100,000 files in each. This is for ease of transfer to other systems.

### Provenance
* Author: Steve Canham
* Organisation: ECRIN (https://ecrin.org)
* System: Clinical Research Metadata Repository (MDR)
* Project: EOSC Life
* Funding: EU H2020 programme, grant 824087
