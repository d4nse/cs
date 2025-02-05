
using Catalyst;
using Catalyst.Models;
using Mosaik.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace NLPApp
{

    public partial class MainWindow : Window
    {
        private DispatcherTimer progressTimer;
        private bool shouldFillBar = false;
        private int totalTicks = 100;
        private int idleUnderTick = 50;
        private int currentTick = 0;
        private const int timerInterval = 10;

        private LanguageDetector? cld2LangDetector;
        private Pipeline? nlpRu;
        private Pipeline? nlpEn;

        public MainWindow()
        {
            InitializeComponent();
            progressTimer = new();
            progressTimer.Interval = TimeSpan.FromMilliseconds(timerInterval);
            progressTimer.Tick += ProgressTimerTick;
            Loaded += MainWindowLoaded;
        }

        private async void MainWindowLoaded(object? sender, RoutedEventArgs e)
        {
            Catalyst.Models.English.Register();
            Catalyst.Models.Russian.Register();
            Storage.Current = new DiskStorage("catalyst-models");
            cld2LangDetector = await LanguageDetector.FromStoreAsync(Mosaik.Core.Language.Any, Mosaik.Core.Version.Latest, "");
            nlpRu = await Pipeline.ForAsync(Mosaik.Core.Language.Russian);
            nlpEn = await Pipeline.ForAsync(Mosaik.Core.Language.English);
        }

        private void ProgressTimerTick(object? sender, EventArgs e)
        {
            if (!shouldFillBar)
            {
                currentTick++;
                if (currentTick >= idleUnderTick)
                {
                    shouldFillBar = true;
                    currentTick = 0;
                }
            }
            else
            {
                currentTick++;
                progressBar.Value = (currentTick / (double)totalTicks) * 100;
                if (currentTick >= totalTicks)
                {
                    progressTimer.Stop();
                    shouldFillBar = false;
                    ProgressTimerOnElapse();
                }
            }
        }

        private void ProgressTimerOnElapse()
        {
            outputBox.Text = string.Empty;
            string text = inputBox.Text;
            if (cld2LangDetector == null)
            {
                outputBox.Text += "cld2 Language detector not loaded yet.\n";
                return;
            }
            if (nlpRu == null || nlpEn == null)
            {
                outputBox.Text += "Processing pipeline not loaded yet.\n";
                return;
            }
            var doc = new Document(text);
            cld2LangDetector.Process(doc);
            outputBox.Text += $"Language: {doc.Language}\n";
            if (doc.Language == Mosaik.Core.Language.English)
                nlpEn.ProcessSingle(doc);
            else if (doc.Language == Mosaik.Core.Language.Russian)
                nlpRu.ProcessSingle(doc);
            else
            {
                outputBox.Text += "Language not supported. To get better precision, keep typing.\n";
                return;
            }

            outputBox.Text += $"Length: {doc.Length}\n";
            Dictionary<PartOfSpeech, int> counts = new Dictionary<PartOfSpeech, int>();
            foreach (var tokdat in doc.TokensData)
            {
                foreach (var dat in tokdat)
                {
                    if (counts.ContainsKey(dat.Tag))
                        counts[dat.Tag]++;
                    else
                        counts[dat.Tag] = 1;
                }
            }
            outputBox.Text += "Parts of speech detected:\n";
            foreach (var kvp in counts)
            {
                outputBox.Text += $" - {GetPartOfSpeechName(kvp.Key)}: {kvp.Value}\n";
            }
        }

        private string GetPartOfSpeechName(PartOfSpeech part)
        {
            return part switch
            {
                PartOfSpeech.ADJ => "Adjective",
                PartOfSpeech.ADP => "Adposition",
                PartOfSpeech.ADV => "Adverb",
                PartOfSpeech.AUX => "Auxiliary",
                PartOfSpeech.CCONJ => "Coordinating Conjunction",
                PartOfSpeech.DET => "Determiner",
                PartOfSpeech.INTJ => "Interjection",
                PartOfSpeech.NOUN => "Noun",
                PartOfSpeech.NUM => "Numeral",
                PartOfSpeech.PART => "Particle",
                PartOfSpeech.PRON => "Pronoun",
                PartOfSpeech.PROPN => "Proper Noun",
                PartOfSpeech.PUNCT => "Punctuation",
                PartOfSpeech.SCONJ => "Subordinating Conjunction",
                PartOfSpeech.SYM => "Symbol",
                PartOfSpeech.VERB => "Verb",
                PartOfSpeech.X => "Other",
                _ => "Unknown"
            };
        }

        private void inputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (progressTimer.IsEnabled) progressTimer.Stop();
            currentTick = 0;
            progressBar.Value = 0;
            shouldFillBar = false;
            if (!string.IsNullOrEmpty(inputBox.Text))
                progressTimer.Start();
        }
    }
}