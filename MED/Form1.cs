using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;
//codigo hecho en GLIBYTE por Jesús Ruiz v2.0
namespace MED
{
    public partial class Form1 : Form
    {
        string ruta = "C:/Users/" + System.Windows.Forms.SystemInformation.UserName + "/Documents/MED";
        string name_file;
        string extension;
        string laruta;
        string name_file2;

        //ya no se utiliza
        //metodo crear carpeta v1.0
        /*private void crear_carpeta() {
            if (Directory.Exists(ruta))
            {
                  //YES
            }
            else
            {
                MessageBox.Show("Se creo la carpeta MED",
                    "Mensaje", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                string pathString = System.IO.Path.Combine(ruta);
                System.IO.Directory.CreateDirectory(pathString);
            }   
        }*/

        //ya no se utiliza
        //Metodo crea un numero random  v1.0
        /*private int numrandom() {
            var seed = Environment.TickCount;
            var random = new Random(seed);
            var value = random.Next(100, 9999);
            return value;
        }*/

        //metodo para abrir TXT
        private void abrirtxt() {
            System.Diagnostics.ProcessStartInfo psi = new ProcessStartInfo("notepad.exe",textBox2.Text);
            Process p = Process.Start(psi);
        }
        
        //metodo para abirr cualquier archivo
        private void abrirall()
        {
            System.Diagnostics.ProcessStartInfo psi = new ProcessStartInfo(textBox2.Text);
            Process p = Process.Start(psi);
        }

        public Form1()
        {
            InitializeComponent();

            //ya no se utiliza
            /*crear_carpeta();*/
        }

        //boton abrir v2.0 (errores corregidos y nuevas funciones)
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            //open.InitialDirectory = "c:\\";
            open.Filter = "med files (*.med)|*.med|All files (*.*)|*.*";
            open.FilterIndex = 2;
            //open.RestoreDirectory = true;
            //if para verificar que se preciono ok 
            if (open.ShowDialog() == DialogResult.OK) //se puso para corregir el error 13
            {
                textBox1.Text = open.FileName;
            }

            //capturar la ruta
            if (textBox1.Text!="") {
                laruta = Path.GetDirectoryName(textBox1.Text);
            }

            //captura el nombre
            name_file = Path.GetFileNameWithoutExtension(textBox1.Text);

            //captura la extension
            extension = Path.GetExtension(textBox1.Text);

            //if para validadr la extension
            if (textBox1.Text!="") {
                if (extension != ".med")
                {
                    button3.Enabled = true;
                    button4.Enabled = false;
                    textBox2.Text = laruta + "\\" + name_file + ".med";
                }
                else {
                    button3.Enabled = false;
                    button4.Enabled = true;
                    textBox2.Text = laruta + "\\" + name_file + "_decrypt.txt";
                }
            }
        }

        //boton cambiar ruta v2.0
        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            //tipos de archivos 
            save.Filter = "med files (*.med)|*.med|txt files (*.txt)|*.txt|Adobe PDF (*.pdf)|*.pdf|" +
                "Microsoft Word (*.docx)|*.docx|mp4 (*.mp4)|*.mp4|png (*.png)|*.png|" +
                "jpg (*.jpg)|*.jpg|mp3 (*.mp3)|*.mp3|Comprimido (*.rar)|*.rar|All files (*.*)|*.*";
            save.FilterIndex = 1;
            name_file2 = Path.GetFileNameWithoutExtension(textBox2.Text);

            if (textBox1.Text == "")
            {
                save.FileName = name_file2;
            }
            else {

                save.FileName = name_file2;
                if (extension != ".med")
                {
                    save.FilterIndex = 1;
                }
                else {
                    save.FilterIndex = 2;
                }
            } 

            //if para verificar que se preciono ok 
            if (save.ShowDialog() == DialogResult.OK) //se puso para corregir el error 14
            {
                textBox2.Text = save.FileName;
                
            }
            
                

            
        }

        //boton encriptar v2.0
        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                if (textBox2.Text != "") //valida si la salida no tiene una ruta marcar una alerta
                {
                    encrypt(textBox1.Text, textBox2.Text); //manda los valroes al metodo de encrypt
                    abrirtxt(); //se llama el metodo para txt
                    textBox1.Text = ""; //limpia los campos
                    textBox2.Text = "";
                    textBox3.Text = "";
                }
                else {
                    //msj error
                    MessageBox.Show("Ninguna ruta de salida",
                        "Alerta", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            else {
                //msj error
                MessageBox.Show("Ningun archivo seleccionado",
                    "Alerta", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }


        }


        //metodo encriptar v2.0 (estable)
        private void encrypt(string input, string output) {
            FileStream inStream, outStream;
            CryptoStream CryStream;
            TripleDESCryptoServiceProvider TDC = new TripleDESCryptoServiceProvider();
           // UTF8Encoding u = new UTF8Encoding();
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            byte[] byteHash, byteTexto;
            inStream = new FileStream(input, FileMode.Open, FileAccess.Read);
            outStream = new FileStream(output, FileMode.OpenOrCreate, FileAccess.Write);
            outStream.SetLength(0);

            byteHash = md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(textBox3.Text));
            //byteHash = md5.ComputeHash(u.GetBytes(textBox3.Text));
            byteTexto = File.ReadAllBytes(input);

            md5.Clear();

            TDC.Key = byteHash;
            TDC.Mode = CipherMode.ECB;
            byte[] IV = TDC.IV;

            CryStream = new CryptoStream(outStream, TDC.CreateEncryptor(byteHash, IV), CryptoStreamMode.Write);

            int bytesRead;
            long length, position = 0;
            length = inStream.Length;

            while(position < length){
                bytesRead = inStream.Read(byteTexto, 0, byteTexto.Length);
                CryStream.Write(byteTexto, 0, bytesRead);
                position += bytesRead;
                Console.WriteLine("{0} bytes processed", position);
            }
            CryStream.Close();
            inStream.Close();
            outStream.Close();
        }

        //boton desencriptar v2.0
        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox1.Text!="") {
                if (textBox2.Text != "")
                {
                    decrypt(textBox1.Text, textBox2.Text);
                    abrirall();
                    textBox1.Text = "";
                    textBox2.Text = "";
                    textBox3.Text = "";
                    button3.Enabled = true;
                    button4.Enabled = false;
                }
                else {
                    //error
                    MessageBox.Show("Ninguna ruta de salida",
                        "Alerta", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            else {
                //error
                MessageBox.Show("Ningun archivo seleccionado",
                    "Alerta", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        //metodo desencriptar v2.0 (estable)
        private void decrypt(string input, string output)
        {
            FileStream inStream, outStream;
            CryptoStream CryStream;
            TripleDESCryptoServiceProvider TDC = new TripleDESCryptoServiceProvider();
          //  UTF8Encoding u = new UTF8Encoding();
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            byte[] byteHash, byteTexto;
            inStream = new FileStream(input, FileMode.Open, FileAccess.Read);
            outStream = new FileStream(output, FileMode.OpenOrCreate, FileAccess.Write);
            outStream.SetLength(0);

            byteHash = md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(textBox3.Text));
            //byteHash = md5.ComputeHash(u.GetBytes(textBox3.Text));
            byteTexto = File.ReadAllBytes(input);

            md5.Clear();

            TDC.Key = byteHash;
            TDC.Mode = CipherMode.ECB;
            byte[] IV = TDC.IV;

            CryStream = new CryptoStream(outStream, TDC.CreateDecryptor(byteHash, IV), CryptoStreamMode.Write);
            
            int bytesRead;
            long length, position = 0;
            length = inStream.Length;

            while (position < length)
            {
                bytesRead = inStream.Read(byteTexto, 0, byteTexto.Length);
                CryStream.Write(byteTexto, 0, bytesRead);
                position += bytesRead;
                Console.WriteLine("{0} bytes processed", position);

            }
            CryStream.Close();
            inStream.Close();
            outStream.Close();
        }
    }
}
