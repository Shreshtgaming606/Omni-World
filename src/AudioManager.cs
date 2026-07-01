using System;
using System.IO;
using System.Media;
using System.Text;

namespace OmniWorld
{
    public sealed class AudioManager
    {
        private SoundPlayer jump;
        private SoundPlayer collect;
        private SoundPlayer hurt;
        private SoundPlayer stomp;
        private SoundPlayer power;
        private SoundPlayer dash;
        private SoundPlayer bounce;
        private SoundPlayer dataCore;
        private SoundPlayer shield;
        private SoundPlayer slamStart;
        private SoundPlayer slamImpact;
        private SoundPlayer blockBreak;
        private SoundPlayer goal;
        private SoundPlayer bgm;
        private MemoryStream jumpStream;
        private MemoryStream collectStream;
        private MemoryStream hurtStream;
        private MemoryStream stompStream;
        private MemoryStream powerStream;
        private MemoryStream dashStream;
        private MemoryStream bounceStream;
        private MemoryStream dataCoreStream;
        private MemoryStream shieldStream;
        private MemoryStream slamStartStream;
        private MemoryStream slamImpactStream;
        private MemoryStream blockBreakStream;
        private MemoryStream goalStream;
        private MemoryStream bgmStream;
        private bool bgmPlaying;

        public bool Enabled;

        public AudioManager()
        {
            Enabled = true;
            try
            {
                jumpStream = MakeTone(620, 0.08, 0.28);
                collectStream = MakeTone(940, 0.06, 0.24);
                hurtStream = MakeTone(160, 0.16, 0.35);
                stompStream = MakeTone(310, 0.09, 0.35);
                powerStream = MakeTone(720, 0.24, 0.28);
                dashStream = MakeChord(new int[] { 540, 810, 1080 }, 0.11, 0.18);
                bounceStream = MakeChord(new int[] { 380, 570, 760 }, 0.13, 0.22);
                dataCoreStream = MakeChord(new int[] { 520, 780, 1040, 1560 }, 0.22, 0.19);
                shieldStream = MakeChord(new int[] { 660, 990, 1320 }, 0.16, 0.19);
                slamStartStream = MakeTone(220, 0.10, 0.24);
                slamImpactStream = MakeChord(new int[] { 110, 165, 330 }, 0.18, 0.28);
                blockBreakStream = MakeNoiseClick();
                goalStream = MakeChord(new int[] { 420, 560, 700 }, 0.35, 0.20);
                bgmStream = MakeMusicLoop();

                jump = new SoundPlayer(jumpStream);
                collect = new SoundPlayer(collectStream);
                hurt = new SoundPlayer(hurtStream);
                stomp = new SoundPlayer(stompStream);
                power = new SoundPlayer(powerStream);
                dash = new SoundPlayer(dashStream);
                bounce = new SoundPlayer(bounceStream);
                dataCore = new SoundPlayer(dataCoreStream);
                shield = new SoundPlayer(shieldStream);
                slamStart = new SoundPlayer(slamStartStream);
                slamImpact = new SoundPlayer(slamImpactStream);
                blockBreak = new SoundPlayer(blockBreakStream);
                goal = new SoundPlayer(goalStream);
                bgm = new SoundPlayer(bgmStream);
            }
            catch
            {
                Enabled = false;
            }
        }

        public void StartMusic()
        {
            if (!Enabled || bgmPlaying) return;
            TryPlayLooping(bgm);
            bgmPlaying = true;
        }

        public void StopMusic()
        {
            if (!Enabled) return;
            try { bgm.Stop(); }
            catch { }
            bgmPlaying = false;
        }

        public void PlayJump() { TryPlay(jump); }
        public void PlayCollect() { TryPlay(collect); }
        public void PlayHurt() { TryPlay(hurt); }
        public void PlayStomp() { TryPlay(stomp); }
        public void PlayPower() { TryPlay(power); }
        public void PlayDash() { TryPlay(dash); }
        public void PlayBounce() { TryPlay(bounce); }
        public void PlayDataCore() { TryPlay(dataCore); }
        public void PlayShield() { TryPlay(shield); }
        public void PlaySlamStart() { TryPlay(slamStart); }
        public void PlaySlamImpact() { TryPlay(slamImpact); }
        public void PlayBreak() { TryPlay(blockBreak); }
        public void PlayGoal() { TryPlay(goal); }

        private void TryPlay(SoundPlayer player)
        {
            if (!Enabled || player == null) return;
            try
            {
                if (player.Stream != null) player.Stream.Position = 0;
                player.Stop();
                player.Play();
            }
            catch
            {
                Enabled = false;
            }
        }

        private void TryPlayLooping(SoundPlayer player)
        {
            if (!Enabled || player == null) return;
            try
            {
                if (player.Stream != null) player.Stream.Position = 0;
                player.PlayLooping();
            }
            catch
            {
                Enabled = false;
            }
        }

        private static MemoryStream MakeTone(int frequency, double seconds, double volume)
        {
            const int sampleRate = 22050;
            int samples = (int)(sampleRate * seconds);
            short[] data = new short[samples];

            for (int i = 0; i < samples; i++)
            {
                double t = (double)i / sampleRate;
                double fade = 1.0 - (double)i / samples;
                data[i] = (short)(Math.Sin(2.0 * Math.PI * frequency * t) * short.MaxValue * volume * fade);
            }

            return ToWave(data, sampleRate);
        }

        private static MemoryStream MakeChord(int[] frequencies, double seconds, double volume)
        {
            const int sampleRate = 22050;
            int samples = (int)(sampleRate * seconds);
            short[] data = new short[samples];

            for (int i = 0; i < samples; i++)
            {
                double t = (double)i / sampleRate;
                double wave = 0.0;
                for (int f = 0; f < frequencies.Length; f++)
                {
                    wave += Math.Sin(2.0 * Math.PI * frequencies[f] * t);
                }

                wave /= frequencies.Length;
                double fade = 1.0 - (double)i / samples;
                data[i] = (short)(wave * short.MaxValue * volume * fade);
            }

            return ToWave(data, sampleRate);
        }

        private static MemoryStream MakeNoiseClick()
        {
            const int sampleRate = 22050;
            int samples = (int)(sampleRate * 0.08);
            short[] data = new short[samples];
            Random random = new Random(8);

            for (int i = 0; i < samples; i++)
            {
                double fade = 1.0 - (double)i / samples;
                data[i] = (short)((random.NextDouble() * 2.0 - 1.0) * short.MaxValue * 0.22 * fade);
            }

            return ToWave(data, sampleRate);
        }

        private static MemoryStream MakeMusicLoop()
        {
            const int sampleRate = 22050;
            double seconds = 4.0;
            int samples = (int)(sampleRate * seconds);
            short[] data = new short[samples];
            int[] notes = new int[] { 262, 330, 392, 330, 294, 370, 440, 370 };
            int noteSamples = samples / notes.Length;

            for (int i = 0; i < samples; i++)
            {
                int note = notes[(i / noteSamples) % notes.Length];
                double t = (double)i / sampleRate;
                double pulse = Math.Sin(2.0 * Math.PI * note * t) > 0.0 ? 1.0 : -1.0;
                double bass = Math.Sin(2.0 * Math.PI * (note / 2) * t) * 0.45;
                double envelope = 0.55 + 0.45 * Math.Sin(2.0 * Math.PI * 2.0 * t);
                data[i] = (short)((pulse * 0.16 + bass * 0.12) * envelope * short.MaxValue);
            }

            return ToWave(data, sampleRate);
        }

        private static MemoryStream ToWave(short[] samples, int sampleRate)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            int dataLength = samples.Length * 2;

            WriteAscii(writer, "RIFF");
            writer.Write(36 + dataLength);
            WriteAscii(writer, "WAVE");
            WriteAscii(writer, "fmt ");
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)1);
            writer.Write(sampleRate);
            writer.Write(sampleRate * 2);
            writer.Write((short)2);
            writer.Write((short)16);
            WriteAscii(writer, "data");
            writer.Write(dataLength);

            for (int i = 0; i < samples.Length; i++)
            {
                writer.Write(samples[i]);
            }

            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private static void WriteAscii(BinaryWriter writer, string value)
        {
            writer.Write(Encoding.ASCII.GetBytes(value));
        }
    }
}
