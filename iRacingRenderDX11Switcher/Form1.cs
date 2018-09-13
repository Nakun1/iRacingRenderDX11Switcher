namespace iRacingRenderDX11Switcher
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    /// <summary>
    /// L'appli en elle même, de type Winform juste pour avoir les messagebox.
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// </summary>
        public Form1()
        {
            this.InitializeComponent();

            // Chemin du fichier de configuration
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var pathfileIni = Path.Combine(path, @"iRacing\rendererDX11.ini");
            var pathfileJeux = Path.Combine(path, @"iRacing\rendererDX11.jeux");
            var pathfileReplay = Path.Combine(path, @"iRacing\rendererDX11.replays");

            Mode mode = Mode.None;

            try
            {
                // Si le fichier .ini n'existe pas => erreur
                if (!File.Exists(pathfileIni))
                {
                    throw new Exception("rendererDX11.ini absent");
                }

                // Si on a un .ini et un .jeux, on passe en mode jeux
                if (File.Exists(pathfileJeux)
                    && !File.Exists(pathfileReplay))
                {
                    File.Move(pathfileIni, pathfileReplay);
                    File.Move(pathfileJeux, pathfileIni);
                    mode = Mode.Jeux;
                }
                // Si on a un .ini et un .replay, on passe en mode replay
                else if (!File.Exists(pathfileJeux)
                        && File.Exists(pathfileReplay))
                {
                    File.Move(pathfileIni, pathfileJeux);
                    File.Move(pathfileReplay, pathfileIni);
                    mode = Mode.Replay;
                }
                // Si on a un .replay et un .jeu, soit il y'a un bug soit c'est l'utilisateur le bug
                else if (File.Exists(pathfileJeux)
                        && File.Exists(pathfileReplay))
                {
                    throw new Exception("Vous ne devez pas avoir un rendererDX11.jeux ET un rendererDX11.replay");
                }
                // Dans les autres cas on a qu'un .ini, on va le dupliquer en .jeux pour faciliter la vie de l'utilisateur.
                else
                {
                    File.Copy(Path.Combine(path, pathfileIni), Path.Combine(path, pathfileJeux));
                    MessageBox.Show("rendererDX11.ini dupliqué en rendererDX11.jeux");
                }

                MessageBox.Show("Nouveau mode: " + mode);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Fichier non trouvé dans " + path);
            }
            catch (Exception e)
            {
                MessageBox.Show("Erreur." + e);
            }

            // On quitte l'appli
            if (System.Windows.Forms.Application.MessageLoop)
            {
                System.Windows.Forms.Application.Exit();
            }
            else
            {
                System.Environment.Exit(1);
            }
        }
    }

    /// <summary>
    /// Liste les périphériques utilisables.
    /// </summary>
    public enum Mode
    {
        None = 0,

        /// <summary>
        /// Oculus Rift ou OpenVR.
        /// </summary>
        Jeux = 1,

        /// <summary>
        /// Écran.
        /// </summary>
        Replay = 2
    }
}
