using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System.IO;
using System;

namespace Szark
{
    public class AudioClip
    {
        private readonly int buffer, channels;
        private readonly int source, bitsPerSample, sampleRate;
        private readonly ALFormat soundFormat;
        private readonly byte[] audioData;

        // Check to see if OpenAL is Installed
        static AudioClip()
        {
            try { new AudioContext(); }
            catch (Exception)
            {
                Debug.Log("OpenAL is not installed on this device!",
                    LogLevel.ERROR);
            }
        }

        public AudioClip(string filePath)
        {
            try
            {
                using (var stream = File.Open(filePath, FileMode.Open))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        string signature = new string(reader.ReadChars(4));
                        if (signature != "RIFF") throw new NotSupportedException();

                        int riff_chunck_size = reader.ReadInt32();

                        string format = new string(reader.ReadChars(4));
                        if (format != "WAVE") throw new NotSupportedException();

                        string format_signature = new string(reader.ReadChars(4));
                        if (format_signature != "fmt ") throw new NotSupportedException();

                        int format_chunk_size = reader.ReadInt32();
                        int audio_format = reader.ReadInt16();
                        channels = reader.ReadInt16();
                        sampleRate = reader.ReadInt32();
                        int byte_rate = reader.ReadInt32();
                        int block_align = reader.ReadInt16();
                        bitsPerSample = reader.ReadInt16();

                        string data_signature = new string(reader.ReadChars(4));
                        if (data_signature != "data") throw new NotSupportedException();
                        int data_chunk_size = reader.ReadInt32();

                        audioData = reader.ReadBytes((int)reader.BaseStream.Length);

                        switch (channels)
                        {
                            case 1:
                                soundFormat = bitsPerSample == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                                break;
                            case 2:
                                soundFormat = bitsPerSample == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                                break;

                            default: throw new NotSupportedException();
                        }

                        buffer = AL.GenBuffer();
                        source = AL.GenSource();

                        AL.BufferData(buffer, soundFormat, audioData, audioData.Length, sampleRate);
                        AL.Source(source, ALSourcei.Buffer, buffer);

                        return;
                    }
                }
            }
            catch (Exception)
            {
                Debug.Log("Sound file not found or not of valid type!",
                    LogLevel.ERROR);
            }
            finally
            {
                Debug.Log("Audio clip could not be successfully created!",
                    LogLevel.ERROR);
            }
        }

        public void Play(float volume = 1)
        {
            AL.Listener(ALListenerf.Gain, volume);
            AL.SourcePlay(source);
        }
    }
}