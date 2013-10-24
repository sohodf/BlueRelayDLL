using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FTD2XX_NET;

namespace BlueRelayControllerDLL
{
    public class RelayCommands
    {
        public FTDI myFtdiDevice = new FTDI();
        FTDI.FT_STATUS ftStatus;
        byte[] sentBytes = new byte[2];
        uint receivedBytes;

        //Opens the com port to the relay
        public void OpenRelay()
        {
            ftStatus = myFtdiDevice.OpenByIndex(0);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                // Wait for a key press
                Console.WriteLine("Failed to open device (error " + ftStatus.ToString() + ")");
                Console.ReadKey();
                return;
            }

            ftStatus = myFtdiDevice.SetBaudRate(921600);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                // Wait for a key press
                Console.WriteLine("Failed to set baudrate (error " + ftStatus.ToString() + ")");
                Console.ReadKey();
                return;
            }

            ftStatus = myFtdiDevice.SetBitMode(255, 4);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                // Wait for a key press
                Console.WriteLine("Failed to set bit mode (error " + ftStatus.ToString() + ")");
                Console.ReadKey();
                return;
            }

            sentBytes[0] = 0;
        }

        //recieves a relay number and turns it on
        //input range - 1-8.
        public void RelayOn(int relayNum)
        {
            int pow = relayNum - 1;
            int command = Power(2, pow);
            byte toSend = (byte)command;
            sentBytes[0] = (byte)(sentBytes[0] | toSend);                                   
            myFtdiDevice.Write(sentBytes, 1, ref receivedBytes);
        }

        //recieves a relay number and turns it off
        //input range - 1-8.
        public void RelayOff(int relayNum)
        {
            int pow = relayNum - 1;
            int command = 255 - Power(2, pow);
            byte toSend = (byte)command;
            sentBytes[0] = (byte)(sentBytes[0] & toSend); 
            myFtdiDevice.Write(sentBytes, 1, ref receivedBytes);
        }

        //send byte
        public void SendByte(byte[] ToSend)
        {
            myFtdiDevice.Write(ToSend, 1, ref receivedBytes);
        }

        //Turns all relays off.
        public void AllRelaysOff()
        {
            sentBytes[0] = (byte)(sentBytes[0] & 0);
            myFtdiDevice.Write(sentBytes, 1, ref receivedBytes);
        }

        //calculates a number to a power of another power
        private int Power(int baseNum, int exp)
        {
            //handle to the power of zero and 1;
            if (exp == 0)
                return 1;
            if (exp == 1)
                return baseNum;
            //higher numbers
            int ret = baseNum;
            for (int i = 1; i < exp; i++)
            {
                ret = ret * baseNum;
            }

            return ret;
        }

    }
}
