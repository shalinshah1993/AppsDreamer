using System;
using Microsoft.Hawaii;
using Microsoft.Hawaii.Speech.Client;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using System.Windows.Threading;
using System.Diagnostics;

namespace PhoneApp2
{
    public partial class MainPage : PhoneApplicationPage
    {
        private List<string> availableGrammars;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.VerifyHawaiiIdentity();

            this.AudioStream = new MemoryStream();
            this.MicroPhone = Microphone.Default;
            this.IsSoundPlaying = false;

            // Timer to simulate the XNA Framework game loop (Microphone is 
            // from the XNA Framework). We also use this timer to monitor the 
            // state of audio playback so we can update the UI appropriately.
            DispatcherTimer dispatchTimer = new DispatcherTimer();
            dispatchTimer.Interval = TimeSpan.FromMilliseconds(33);
            dispatchTimer.Tick += new EventHandler(this.DispatcherTimer_Tick);
            dispatchTimer.Start();

            // Event handler for getting audio data when the buffer is full
            this.MicroPhone.BufferReady += new EventHandler<EventArgs>(this.Microphone_BufferReady);

            SpeechService.GetGrammarsAsync(
                HawaiiClient.HawaiiApplicationId,
                (result) =>
                {
                    this.Dispatcher.BeginInvoke(() => this.OnSpeechGrammarsReceived(result));
                });

        }
        /// <summary>
        /// Delegate definition for SetRecognizedText.
        /// </summary>
        /// <param name="recognizedTexts">
        /// List of recognized results.
        /// </param>
        private delegate void SetRecognizedTextDelegate(List<string> recognizedTexts);

        /// <summary>
        /// Delegate definition for SetGrammars.
        /// </summary>
        private delegate void SetGrammarsDelegate();

        /// <summary>
        /// Gets or sets the object representing the physical microphone on the device.
        /// </summary>
        private Microphone MicroPhone { get; set; }

        /// <summary>
        /// Gets or sets dynamic buffer to retrieve audio data from the microphone.
        /// </summary>
        private byte[] AudioBuffer { get; set; }

        /// <summary>
        /// Gets or sets the audio data for later playback.
        /// </summary>
        private MemoryStream AudioStream { get; set; }

        /// <summary>
        /// Gets or sets the SoundEffect class we need to instantiate the SoundInstance.
        /// </summary>
        private SoundEffect SoundEffect { get; set; }

        /// <summary>
        /// Gets or sets the sound instance to play back audio.
        /// </summary>
        private SoundEffectInstance SoundInstance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the sound is playing.
        /// </summary>
        private bool IsSoundPlaying { get; set; }

        /// <summary>
        /// Verify that the Hawaii identity is set correctly.
        /// </summary>
        private void VerifyHawaiiIdentity()
        {
            if (string.IsNullOrEmpty(HawaiiClient.HawaiiApplicationId))
            {
                MessageBox.Show("Service credentials are not set. Please consult the \"Project Hawaii Installation Guide\" on how to obtain credentials");
                throw new Exception("Credentials are not set, exiting application");
            }
        }

        /// <summary>
        /// Updates the XNA FrameworkDispatcher and checks to see if a sound is playing.
        /// If sound has stopped playing, it updates the UI.
        /// </summary>
        /// <param name="sender">
        /// Sender of this event.
        /// </param>
        /// <param name="e">
        /// Event arguments.
        /// </param>
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                FrameworkDispatcher.Update();
            }
            catch
            {
            }

            if (true == this.IsSoundPlaying)
            {
                if (this.SoundInstance.State != SoundState.Playing)
                {
                    // Audio has finished playing
                    this.IsSoundPlaying = false;

                }
            }
        }

        /// <summary>
        /// Event handler for buffer ready.
        /// </summary>
        /// <param name="sender">
        /// Sender of this event.
        /// </param>
        /// <param name="e">
        /// Event arguments.
        /// </param>
        private void Microphone_BufferReady(object sender, EventArgs e)
        {
            // Retrieve audio data
            this.MicroPhone.GetData(this.AudioBuffer);

            // Store the audio data in a stream
            this.AudioStream.Write(this.AudioBuffer, 0, this.AudioBuffer.Length);
        }

       
        private void speak_Click(object sender, RoutedEventArgs e)
        {
            // Get audio data in 1/2 second chunks
            this.MicroPhone.BufferDuration = TimeSpan.FromMilliseconds(500);

            // Allocate memory to hold the audio data
            this.AudioBuffer = new byte[this.MicroPhone.GetSampleSizeInBytes(this.MicroPhone.BufferDuration)];

            // Set the stream back to zero in case there is already something in it
            this.AudioStream.SetLength(0);

            // Start recording
            this.MicroPhone.Start();
        }     /// <summary>

        /// <summary>
        /// Speech Grammars Received handler.
        /// </summary>
        /// <param name="result">
        /// Service Result.
        /// </param>
        private void OnSpeechGrammarsReceived(SpeechServiceResult result)
        {
            Debug.Assert(result != null, "result is null");

            if (result.Status == Status.Success)
            {

                this.availableGrammars = result.SpeechResult.Items;
                if (this.availableGrammars == null)
                {
                    return;
                }
            }
            else
            {
                string error = "Error receiving available speech grammars.";
                if (result.Exception != null && result.Exception.Message.Contains("The appId"))
                {
                    // Here we do not show the error message directly because it would expose the appid to users.
                    error = "Error receiving available speech grammars. The Hawaii app Id is invalid!";
                }
                else
                {
                    error = "Error receiving available speech grammars.";
                }


                MessageBox.Show(error, "Error", MessageBoxButton.OK);
            }
        }
        /// <summary>
        /// Speech recognition completed handler.
        /// </summary>
        /// <param name="speechResult">
        /// Service result.
        /// </param>
        private void OnSpeechRecognitionCompleted(SpeechServiceResult speechResult)
        {
            Debug.Assert(speechResult != null, "speechResult is null");
            

            if (speechResult.Status == Status.Success)
            {
                //this.SetRecognizedTextListBox(speechResult.SpeechResult.Items);
                textBlock1.Text = speechResult.SpeechResult.Items.ToString();
            }
            else
            {
                if (speechResult.Exception == null)
                {
                    MessageBox.Show("Error recognizing the speech.", "Error", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show(speechResult.Exception.Message, "Error", MessageBoxButton.OK);
                }
            }
        }

        

        private void stop_Click(object sender, RoutedEventArgs e)
        {   try{
            if (this.MicroPhone.State == MicrophoneState.Started)
            {
                // In RECORD mode, user clicked the 
                // stop button to end recording
                this.MicroPhone.Stop();
            }
            else if (this.SoundInstance.State == SoundState.Playing)
            {
                // In PLAY mode, user clicked the 
                // stop button to end playing back
                this.SoundInstance.Stop();
            }
            if (this.AudioStream != null && this.AudioStream.Length != 0)
            {
                SpeechService.RecognizeSpeechAsync(
                HawaiiClient.HawaiiApplicationId,
                   "Dictation",
                this.AudioStream.ToArray(),
                (result) =>
                {
                    this.Dispatcher.BeginInvoke(() => this.OnSpeechRecognitionCompleted(result));
                });
            }
            else
            {
                MessageBox.Show("Invalid speech buffer found. Record speech and try again.", "Error", MessageBoxButton.OK);
            }
        }
            catch(Exception ewe12){
            
            }
            


        }

    }
}