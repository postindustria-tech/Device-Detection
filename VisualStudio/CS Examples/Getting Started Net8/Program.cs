/* *********************************************************************
 * This Source Code Form is copyright of 51Degrees Mobile Experts Limited.
 * Copyright 2017 51Degrees Mobile Experts Limited, 5 Charlotte Close,
 * Caversham, Reading, Berkshire, United Kingdom RG4 7BY
 *
 * This Source Code Form is the subject of the following patents and patent
 * applications, owned by 51Degrees Mobile Experts Limited of 5 Charlotte
 * Close, Caversham, Reading, Berkshire, United Kingdom RG4 7BY:
 * European Patent No. 2871816;
 * European Patent Application No. 17184134.9;
 * United States Patent Nos. 9,332,086 and 9,350,823; and
 * United States Patent Application No. 15/686,066.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0.
 *
 * If a copy of the MPL was not distributed with this file, You can obtain
 * one at http://mozilla.org/MPL/2.0/.
 *
 * This Source Code Form is "Incompatible With Secondary Licenses", as
 * defined by the Mozilla Public License, v. 2.0.
 ********************************************************************** */

/*
<tutorial>
Getting started example of using 51Degrees device detection. The example shows 
how to:
<ol>
    <li>Initialise detector with path to the 51Degrees device data file and 
    a list of properties.
    <p><code>
        string properties = "IsMobile";<br />
        string fileName = args[0];<br />
        Provider provider = new Provider(FileName, properties);
    </code></p>
    <li>Match a single HTTP User-Agent header string.
    <p><code>
        Match match;
        using (match = provider.getMatch(userAgent)) {
            // Do something with match result.
        }
    </code></p>
    <li>Extract the value of the IsMobile property.
    <p><code>
        string IsMobile;
        IsMobile = match.getValue("IsMobile");
    </code></p>
    <li> Dispose of the Provider releasing the resources.
    <p><code>
        provider.Dispose();
    </code></p>
</ol>
<p>
    This tutorial assumes you are building this example using Visual Studio. 
    You should supply path to the 51Degrees device data file that you wish to 
    use as a command line argument.
</p>
<p>
    By default the API is distributed with Lite data which is free to use for 
    both the non-commercial and commercial purposes. Lite data file contains 
    over 60 properties. For more properties like DeviceType, PriceBand and 
    HardwareVendor check out the Premium and Enterprise data files:
    https://51degrees.com/compare-data-options
</p>
<p>
    Passing a list of properties to the provider constructor limits the number 
    of properties in the dataset the provider wraps to only the chosen 
    properties. Not providing a list or providing an empty list will cause the 
    dataset to be created with all available properties:
    <br /><code>
        Provider provider = new Provider(fileName, "");
        Provider provider = new Provider(fileName);
    </code>
</p>
<p>
    This code works with the assembly generated by SWIG from the 51Degrees C 
    source files. The garbage collection does not apply to the SWIG-generated 
    code. You should always use the Dispose() methods on any objects that 
    originate from the unmanaged code.
</p>
<p>
    Match retrieves the workset from the pool of worksets and not disposing 
    of the Match objects will eventually exhaust the pool and cause the 
    program to stall as it waits for the free workset. We recommend you either 
    use Match object in the context of a using statement or within the try 
    finally block where the match is disposed of if not null.
</p>
<p>
    Provider wraps the dataset which is the data file loaded into memory and 
    ready to be used through the methods exposed by the Provider. Failing to 
    dispose of the provider when it is no longer required means the memory 
    allocated for the provider, cache, pool and the dataset will never be 
    released.
</p>
</tutorial>
*/
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FiftyOne.Mobile.Detection.Provider.Interop.Pattern;
using System.Diagnostics;

namespace FiftyOne.Example.Illustration.CSharp.GettingStarted
{
    public class Program
    {
        // Snippet Start
        /// <summary>
        /// Runs the program.
        /// </summary>
        /// <param name="fileName">
        /// Path to the 51Degrees device data file.
        /// </param>
        public static void Run(string fileName)
        {
            // Comma-separated list of properties.
            string properties = "IsMobile";
            // User-Agent string of an iPhone mobile device.
            string mobileUserAgent = ("Mozilla/5.0 (iPhone; CPU iPhone " +
                "OS 7_1 like Mac OS X) AppleWebKit/537.51.2 (KHTML, like " +
                "Gecko) 'Version/7.0 Mobile/11D167 Safari/9537.53");
            // User-Agent string of Firefox Web browser version 41 on desktop.
            string desktopUserAgent = ("Mozilla/5.0 (Windows NT 6.3; " +
                "WOW64; rv:41.0) Gecko/20100101 Firefox/41.0");
            // User-Agent string of a MediaHub device.
            string mediaHubUserAgent = ("Mozilla/5.0 (Linux; Android " +
                "4.4.2; X7 Quad Core Build/KOT49H) AppleWebKit/537.36 " +
                "(KHTML, like Gecko) Version/4.0 Chrome/30.0.0.0 " +
                "Safari/537.36");
            // Use path to the data file and a list of properties to create 
            // provider.
            Provider provider = new Provider(fileName, properties);

            Console.WriteLine("Starting Getting Started Example.");

            // Carries out a match for a mobile User-Agent.
            Console.WriteLine("\nMobile User-Agent: " + mobileUserAgent);
            detect(provider, mobileUserAgent, "True");

            // Carries out a match for a desktop User-Agent.
            Console.WriteLine("\nDesktop User-Agent: " + desktopUserAgent);
            detect(provider, desktopUserAgent, "False");

            // Carries out a match for a MediaHub User-Agent.
            Console.WriteLine("\nMediaHub User-Agent: " + mediaHubUserAgent);
            detect(provider, mediaHubUserAgent, "False");

            // At the end of the program dispose of the data file to 
            // deallocate memory.
            provider.Dispose();
        }

        /// <summary>
        /// Performs detection by invoking the getMatch method of the provider 
        /// and disposing of the match result object after printing result and 
        /// performing Assert.
        /// </summary>
        /// <remarks>
        /// When working with Match objects you should always call dispose in 
        /// order to free the match and return the workset to the pool of 
        /// worksets.
        /// </remarks>
        /// <param name="provider">
        /// FiftyOne Provider that enables methods to interact with the 
        /// 51Degrees device data file.
        /// </param>
        /// <param name="userAgent">
        /// A string containing the HTTP User-Agent to identify.
        /// </param>
        /// <param name="expected">
        /// Used for test purposes only. Contains the expected value for the 
        /// IsMobile property.
        /// </param>
        public static void detect(Provider provider, 
                                  string userAgent, 
                                  string expected)
        {
            Match match;
            string IsMobile;

            using (match = provider.getMatch(userAgent))
            {
                IsMobile = match.getValue("IsMobile");
                Debug.Assert(expected == IsMobile, expected, IsMobile);
                Console.WriteLine("   IsMobile: " + IsMobile);
            }
        }
        // Snippet End

        static void Main(string[] args)
        {
            string fileName = args.Length > 0 ? args[0] : 
                "../../../../../../../data/51Degrees-LiteV3.2.dat";
            Run(fileName);

            // Wait for a character to be pressed.
            Console.ReadKey();
        }
    }
}
