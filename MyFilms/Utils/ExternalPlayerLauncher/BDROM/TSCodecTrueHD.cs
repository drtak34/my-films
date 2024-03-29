﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BDInfo
{
    public abstract class TSCodecTrueHD
    {
        public static void Scan(
            TSAudioStream stream,
            TSStreamBuffer buffer,
            ref string tag)
        {
            if (stream.IsInitialized &&
                stream.CoreStream != null &&
                stream.CoreStream.IsInitialized) return;

            bool syncFound = false;
            uint sync = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                sync = (sync << 8) + buffer.ReadByte();
                if (sync == 0xF8726FBA) 
                {
                    syncFound = true;
                    break;
                }
            }

            if (!syncFound)
            {
                tag = "CORE";
                if (stream.CoreStream == null)
                {
                    stream.CoreStream = new TSAudioStream();
                    stream.CoreStream.StreamType = TSStreamType.AC3_AUDIO;
                }
                if (!stream.CoreStream.IsInitialized)
                {
                    buffer.BeginRead();
                    TSCodecAC3.Scan(stream.CoreStream, buffer, ref tag);
                }
                return;
            }

            tag = "HD";
            int ratebits = buffer.ReadBits(4);
            if (ratebits != 0xF)
            {
                stream.SampleRate = 
                    (((ratebits & 8) > 0 ? 44100 : 48000) << (ratebits & 7));
            }
            int temp1 = buffer.ReadBits(8);
            int channels_thd_stream1 = buffer.ReadBits(5);
            int temp2 = buffer.ReadBits(2);

            stream.ChannelCount = 0;
            stream.LFE = 0;
            int c_LFE2 = buffer.ReadBits(1);
            if (c_LFE2 == 1)
            {
                stream.LFE += 1;
            }
            int c_Cvh = buffer.ReadBits(1);
            if (c_Cvh == 1)
            {
                stream.ChannelCount += 1;
            }
            int c_LRw = buffer.ReadBits(1);
            if (c_LRw == 1)
            {
                stream.ChannelCount += 2;
            }
            int c_LRsd = buffer.ReadBits(1);
            if (c_LRsd == 1)
            {
                stream.ChannelCount += 2;
            }
            int c_Ts = buffer.ReadBits(1);
            if (c_Ts == 1)
            {
                stream.ChannelCount += 1;
            }
            int c_Cs = buffer.ReadBits(1);
            if (c_Cs == 1)
            {
                stream.ChannelCount += 1;
            }
            int c_LRrs = buffer.ReadBits(1);
            if (c_LRrs == 1)
            {
                stream.ChannelCount += 2;
            }
            int c_LRc = buffer.ReadBits(1);
            if (c_LRc == 1)
            {
                stream.ChannelCount += 2;
            }
            int c_LRvh = buffer.ReadBits(1);
            if (c_LRvh == 1)
            {
                stream.ChannelCount += 2;
            }
            int c_LRs = buffer.ReadBits(1);
            if (c_LRs == 1)
            {
                stream.ChannelCount += 2;
            }
            int c_LFE = buffer.ReadBits(1);
            if (c_LFE == 1)
            {
                stream.LFE += 1;
            }
            int c_C = buffer.ReadBits(1);
            if (c_C == 1)
            {
                stream.ChannelCount += 1;
            }
            int c_LR = buffer.ReadBits(1);
            if (c_LR == 1)
            {
                stream.ChannelCount += 2;
            }

            int access_unit_size = 40 << (ratebits & 7);
            int access_unit_size_pow2 = 64 << (ratebits & 7);

            buffer.ReadBits(24);
            buffer.ReadBits(24);

            int is_vbr = buffer.ReadBits(1);
            int peak_bitrate = buffer.ReadBits(15);
            peak_bitrate = (peak_bitrate * stream.SampleRate) >> 4;

            double peak_bitdepth = 
                (double)peak_bitrate / 
                (stream.ChannelCount + stream.LFE) / 
                stream.SampleRate;
            if (peak_bitdepth > 14)
            {
                stream.BitDepth = 24;
            }
            else
            {
                stream.BitDepth = 16;
            }

#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format(
                "{0}\t{1}\t{2:F2}", 
                stream.PID, peak_bitrate, peak_bitdepth));
#endif
            stream.IsVBR = true;
            stream.IsInitialized = true;
        }
    }
}
