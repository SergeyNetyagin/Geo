using System;
using System.IO;

namespace RemoveUnityTrialVersion {

    internal class Packer {

        public Action<string> outLog;
        readonly static string watermark = "UnityWatermark-trial";
        public bool init = false;

        public void RemoveWaterMark() {

            if( !init ) {
            
				return;
			}

            outLog( "Removing \"Trial Version\" Watermark. . ." );

            FileStream file = new FileStream( @"Resources\unity default resources", FileMode.Open );
            
			file.Seek( 0, SeekOrigin.Begin );
            
			int b = 0, c = 0;

            outLog( "Processing ...\r" );
			
			while( true && file.Position < file.Length ) {

				b = file.ReadByte();

				if( c >= watermark.Length ) {

					if( b != 45 ) {
					
						break;
					}

					else {
						
						c = 0;
					}
				}

				if( b == watermark[c] ) {

					c++;
				}

				else {

					c = 0;
				}
			}

			c = 112;

			while( c > 0 && file.Position < file.Length ) {

				file.WriteByte(0);
				
				c--;
			}
            
            file.Flush(true);
            file.Close();

            outLog( "Watermark has been removed" );
        }
    }
}