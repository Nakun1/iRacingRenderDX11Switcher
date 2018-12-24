namespace iRacingRenderDX11Switcher
{
    using System;
    using System.IO;
    using System.Linq;
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

            // Noms des fichiers
            var pathRenderIni = Path.Combine(path, @"iRacing\rendererDX11.ini");
            var pathRenderVR = Path.Combine(path, @"iRacing\rendererDX11.VR");
            var pathRenderEcran = Path.Combine(path, @"iRacing\rendererDX11.Ecran");
            var pathAppIni = Path.Combine(path, @"iRacing\app.ini");
            var pathAppVR = Path.Combine(path, @"iRacing\app.VR");
            var pathAppEcran = Path.Combine(path, @"iRacing\app.Ecran");

            Mode mode = Mode.None;

            try
            {
                // Si le fichier .ini n'existe pas => erreur
                if (!File.Exists(pathRenderIni))
                {
                    throw new Exception("rendererDX11.ini absent");
                }

                // Si on a qu'un rendererDX11.ini, on va le dupliquer en .VR pour faciliter la vie de l'utilisateur.
                if (!File.Exists(pathRenderVR) && !File.Exists(pathRenderEcran))
                {
                    File.Copy(Path.Combine(path, pathRenderIni), Path.Combine(path, pathRenderVR));

                    // On désactive la VR dans le fichier d'origine
                    this.DesactiverVRDansRenderIni(Path.Combine(path, pathRenderIni));

                    // On active la VR dans le fichier créé
                    this.ActiverVRDansRenderIni(Path.Combine(path, pathRenderVR));

                    //MessageBox.Show("rendererDX11.ini dupliqué en rendererDX11.VR");
                }

                // Si on a qu'un app.ini, on va le dupliquer en .VR pour faciliter la vie de l'utilisateur.
                if (!File.Exists(pathAppVR) && !File.Exists(pathAppEcran))
                {
                    File.Copy(Path.Combine(path, pathAppIni), Path.Combine(path, pathAppVR));
                    //MessageBox.Show("app.ini dupliqué en app.VR");
                }

                // Si on a un .ini et un .VR, on passe en mode VR
                if (File.Exists(pathRenderVR) && !File.Exists(pathRenderEcran)
                    && File.Exists(pathAppVR) && !File.Exists(pathAppEcran)
                    )
                {
                    File.Move(pathRenderIni, pathRenderEcran);
                    File.Move(pathRenderVR, pathRenderIni);
                    File.Move(pathAppIni, pathAppEcran);
                    File.Move(pathAppVR, pathAppIni);
                    mode = Mode.VR;
                }
                // Si on a un .ini et un .Ecran, on passe en mode Ecran
                else if (!File.Exists(pathRenderVR) && File.Exists(pathRenderEcran)
                         && !File.Exists(pathAppVR) && File.Exists(pathAppEcran))
                {
                    File.Move(pathRenderIni, pathRenderVR);
                    File.Move(pathRenderEcran, pathRenderIni);
                    File.Move(pathAppIni, pathAppVR);
                    File.Move(pathAppEcran, pathAppIni);
                    mode = Mode.Ecran;
                }
                // Si on a un .Ecran et un .jeu, soit il y'a un bug soit c'est l'utilisateur le bug
                else if (File.Exists(pathRenderVR) && File.Exists(pathRenderEcran)
                         || File.Exists(pathAppVR) && File.Exists(pathAppEcran))
                {
                    throw new Exception("Vous ne devez pas avoir un rendererDX11.VR ET un rendererDX11.Ecran, ni un app.VR ET un app.Ecran");
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

        /// <summary>
        /// Desactive la VR dans le fichier Render.ini.
        /// </summary>
        /// <param name="path">Le chemin complet du fichier.</param>
        private void DesactiverVRDansRenderIni(string path)
        {
            try
            {
                var lignesRendererDX11 = File.ReadAllLines(path);

                for (int i = 0; i < lignesRendererDX11.Count(); i++)
                {
                    if (lignesRendererDX11[i].Length >= 11 && lignesRendererDX11[i].Substring(0, 11) == "RiftEnabled")
                    {
                        lignesRendererDX11[i] = lignesRendererDX11[i].Replace("1", "0");
                    }
                }

                File.WriteAllLines(path, lignesRendererDX11);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Fichier non trouvé dans " + path);
            }
            catch (Exception e)
            {
                MessageBox.Show("Erreur." + e);
            }
        }

        /// <summary>
        /// Active la VR dans le fichier Render.ini.
        /// </summary>
        /// <param name="path">Le chemin complet du fichier.</param>
        private void ActiverVRDansRenderIni(string path)
        {
            try
            {
                var lignesRendererDX11 = File.ReadAllLines(path);

                for (int i = 0; i < lignesRendererDX11.Count(); i++)
                {
                    if (lignesRendererDX11[i].Length >= 11 && lignesRendererDX11[i].Substring(0, 11) == "RiftEnabled")
                    {
                        lignesRendererDX11[i] = lignesRendererDX11[i].Replace("0", "1");
                    }
                }

                File.WriteAllLines(path, lignesRendererDX11);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Fichier non trouvé dans " + path);
            }
            catch (Exception e)
            {
                MessageBox.Show("Erreur." + e);
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
        VR = 1,

        /// <summary>
        /// Écran.
        /// </summary>
        Ecran = 2
    }
}
