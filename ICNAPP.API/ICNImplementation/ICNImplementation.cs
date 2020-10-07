using ICNAPP.CSImplementation.Interface;
using ICNAPP.CSImplementation.Operations;
using ICNAPP.Forwarder.Interface;
using ICNAPP.Forwarder.Operations;
using ICNAPP.Models.CustomModels;
using ICNAPP.Models.DatabaseModel;
using ICNAPP.PITEntries.Interface;
using ICNAPP.PITEntries.Operations;
using ICNAPPHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace ICNAPP.API.ICNImplementation
{
    public class ICNImplementation
    {
        private IContentStoreService _contentStoreService;
        private IPITEntriesService _pitService;
        private IFIBEntriesService _fibService;

        public ICNImplementation()
        {
            _contentStoreService = new ContentStoreService();
            _pitService = new PITEntriesService();
            _fibService = new FIBEntriesService();
        }

        public string ProcessInterest(string interestName, string face)
        {
            try
            {
                PITEntry pITEntry = _pitService.GetPITEntryBy(interestName);
                if (pITEntry != null)
                {
                    pITEntry.IncommingFace.Add(face);
                    _pitService.EditPITEntry(pITEntry);
                }
                else // PIT does not exist check content store
                {
                    ContentStore contentStore = _contentStoreService.GetContentStoreBy(interestName);
                    if (contentStore != null)
                    {
                        return contentStore.Content;
                    }
                    else // content is not in CS
                    {
                        FIBEntry fIBEntry = _fibService.GetFIBEntryBy(interestName);
                        ForwardInterest(fIBEntry, interestName);
                    }
                }
                return string.Empty;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string ProcessData(string interestName, string data, string face)
        {
            try
            {
                PITEntry pITEntry = _pitService.GetPITEntryBy(interestName);
                if (pITEntry != null)
                {
                    ForwardData(interestName, data, pITEntry);
                }
                else // PIT does not exist - data unsolicited
                {
                    // drop
                    return string.Empty;
                }
                return string.Empty;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void ForwardData(string interestName, string data, PITEntry pITEntry)
        {
            string fileName = ConfigurationHelper.ProcessDataFilePath+DateTime.Now.Ticks;

            try
            {
                // Check if file already exists. If yes, delete it.     
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                // Create a new file     
                using (FileStream fs = File.Create(fileName))
                {
                    // Add some text to file    
                    Byte[] title = new UTF8Encoding(true).GetBytes("Process Data");
                    fs.Write(title, 0, title.Length);
                    byte[] interestNameInfo = new UTF8Encoding(true).GetBytes(interestName);
                    fs.Write(interestNameInfo, 0, interestNameInfo.Length);
                    Byte[] dataInfo = new UTF8Encoding(true).GetBytes(data);
                    fs.Write(dataInfo, 0, dataInfo.Length);
                    byte[] incomingFaceInfo = new UTF8Encoding(true).GetBytes(pITEntry.IncommingFace.ToString());
                    fs.Write(incomingFaceInfo, 0, incomingFaceInfo.Length);
                    Byte[] OutGoingFace = new UTF8Encoding(true).GetBytes(pITEntry.OutGoingFace);
                    fs.Write(OutGoingFace, 0, OutGoingFace.Length);

                }

               
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }

        }
private void ForwardNACK(string interestName, string data, PITEntry pITEntry)
        {
            string fileName = ConfigurationHelper.ProcessDataFilePath+DateTime.Now.Ticks;

            try
            {
                // TODO

               
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }

        }
        private void ForwardInterest(FIBEntry fIBEntry, string interestName)
        {
            string fileName = ConfigurationHelper.ProcessInterestFilePath + DateTime.Now.Ticks;

            try
            {
                // Check if file already exists. If yes, delete it.     
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                // Create a new file     
                using (FileStream fs = File.Create(fileName))
                {
                    // Add some text to file    
                    Byte[] title = new UTF8Encoding(true).GetBytes("Process Interest");
                    fs.Write(title, 0, title.Length);
                    byte[] interestNameInfo = new UTF8Encoding(true).GetBytes(interestName);
                    fs.Write(interestNameInfo, 0, interestNameInfo.Length);
                    Byte[] fibInfo = new UTF8Encoding(true).GetBytes(fIBEntry.FaceInformation);
                    fs.Write(fibInfo, 0, fibInfo.Length);

                }


            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }
    }
}
