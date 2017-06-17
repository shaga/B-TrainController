using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Globalization;
using Windows.Media.SpeechRecognition;
using Windows.UI.Core;

namespace BTrainDemoApp.Models
{
    public enum EVoiceCommand
    {
        None,
        Go,
        Stop,
    }

    class VoiceCommandReceiver
    {
        #region const

        private const string PathGrammarFile = "Data\\VoiceCommandGrammar.xml";

        private const string SpeechTag = "cmd";

        #endregion

        #region field

        private readonly SpeechRecognizer _recognizer;

        #endregion

        #region property

        private static CoreDispatcher Dispatcher => CoreApplication.MainView.Dispatcher;

        private bool _isInitialized;

        #endregion

        #region event

        public event EventHandler<EVoiceCommand> VoiceCommandReceived;

        #endregion

        #region constructor

        public VoiceCommandReceiver()
        {
            _recognizer = new SpeechRecognizer(new Language("ja"));
            _recognizer.StateChanged += OnRecognizerStateChanged;
            _recognizer.ContinuousRecognitionSession.ResultGenerated += OnRecognizerGeneratedResult;
        }

        #endregion

        #region method

        public async Task<bool> Initialize()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, async () =>
            {
                if (_isInitialized)
                {
                    await _recognizer.StopRecognitionAsync();
                    _recognizer.Constraints.Clear();
                }

                var grammarFile = await Package.Current.InstalledLocation.GetFileAsync(PathGrammarFile);
                var grammarConstraint = new SpeechRecognitionGrammarFileConstraint(grammarFile);

                _recognizer.Constraints.Add(grammarConstraint);

                var result = await _recognizer.CompileConstraintsAsync();

                _isInitialized = result.Status == SpeechRecognitionResultStatus.Success;

                if (_isInitialized)
                {
                    await _recognizer.ContinuousRecognitionSession.StartAsync();
                }
            });

            return _isInitialized;
        }

        #region SpeechRecognizer event handler

        private void OnRecognizerStateChanged(SpeechRecognizer recognizer, SpeechRecognizerStateChangedEventArgs e)
        {
            
        }

        private void OnRecognizerGeneratedResult(SpeechContinuousRecognitionSession session,
            SpeechContinuousRecognitionResultGeneratedEventArgs e)
        {
            if (!e.Result.SemanticInterpretation.Properties.ContainsKey(SpeechTag)) return;

            var tag = e.Result.SemanticInterpretation.Properties[SpeechTag].LastOrDefault();

            var command = EVoiceCommand.None;

            if (string.IsNullOrEmpty(tag) || !Enum.TryParse(tag, true, out command) || command == EVoiceCommand.None)
            {
                return;
            }

            VoiceCommandReceived?.Invoke(this, command);
        }

        #endregion

        #endregion
    }
}
