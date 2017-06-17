using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Media.SpeechRecognition;

namespace BTrainController.Models
{
    internal enum ECommand
    {
        None,
        Go,
        Stop,
    }

    internal class VoiceCommand
    {
        #region const

        private const string UriCommandGrammar = "Grammar\\CommandGrammar.xml";

        private const string SpeechTag = "cmd";
        private const string SpeechTagStart = "Start";

        #endregion

        #region field

        private readonly SpeechRecognizer _recognizer;

        private bool _isStarted;

        #endregion

        #region property



        #endregion
        
        #region event

        public event EventHandler<bool> InitializedStateChanged;

        public event EventHandler<ECommand> ReceivedCommand;
        
        #endregion
        
        #region constructor

        public VoiceCommand()
        {
            _recognizer = new SpeechRecognizer();
            _recognizer.StateChanged += OnRecognizerStateChanged;
            _recognizer.ContinuousRecognitionSession.ResultGenerated += OnRecognizerGeneratedResult;
        }

        #endregion

        #region method

        public async void Initialize()
        {
            if (_isStarted)
            {
                await _recognizer.StopRecognitionAsync();
            }

            var grammarFile = await Package.Current.InstalledLocation.GetFileAsync(UriCommandGrammar);

            var grammarConstraint = new SpeechRecognitionGrammarFileConstraint(grammarFile);

            _recognizer.Constraints.Add(grammarConstraint);

            var result = await _recognizer.CompileConstraintsAsync();

            _isStarted = result.Status == SpeechRecognitionResultStatus.Success;

            if (_isStarted)
            {
                await _recognizer.ContinuousRecognitionSession.StartAsync();
            }

            InitializedStateChanged?.Invoke(this, _isStarted);
        }

        private void OnRecognizerStateChanged(SpeechRecognizer recognizer, SpeechRecognizerStateChangedEventArgs e)
        {
            
        }

        private void OnRecognizerGeneratedResult(SpeechContinuousRecognitionSession session,
            SpeechContinuousRecognitionResultGeneratedEventArgs e)
        {
            if (!e.Result.SemanticInterpretation.Properties.ContainsKey(SpeechTag)) return;

            if (!e.Result.SemanticInterpretation.Properties[SpeechTag].Any(v => v.Equals(SpeechTagStart))) return;

            ReceivedCommand?.Invoke(this, ECommand.Go);
        }

        #endregion
    }
}
