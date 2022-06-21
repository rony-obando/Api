﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Libreria
{
    public partial class LibrosForm : Form
    {
        private static int id = 0;

        public LibrosForm()
        {
            InitializeComponent();
        }

        private void LibrosForm_Load(object sender, EventArgs e)
        {
            GetAllLibros();
        }

        private async void GetAllLibros()
        {
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync("https://localhost:44343/api/Libros"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var LibrosJsonString = await response.Content.ReadAsStringAsync();
                        dgvLibros.DataSource = JsonConvert.DeserializeObject<List<ViewModels.LibrosViewModel>>(LibrosJsonString)
                            .ToList();
                    }
                    else
                    {
                        MessageBox.Show("No se pueden obtener los libros: " + response.StatusCode);
                    }
                }
            }
        }

        private async void GetLibroById(int id)
        {
            using (var client = new HttpClient())
            {
                string URI = "https://localhost:44343/api/Libros/" + id.ToString();
                HttpResponseMessage response = await client.GetAsync(URI);
                if (response.IsSuccessStatusCode)
                {
                    var LibroJsonString = await response.Content.ReadAsStringAsync();
                    ViewModels.LibrosViewModel oLibro =
                        JsonConvert.DeserializeObject<ViewModels.LibrosViewModel>(LibroJsonString);
                    txtISBN.Text = oLibro.ISBN;
                    txtAutor.Text = oLibro.Autor;
                    txtTemas.Text = oLibro.Temas;
                    txtTitulo.Text = oLibro.Titulo;
                    txtEditorial.Text = oLibro.Editorial;
                }
                else
                    MessageBox.Show("No se puede obtener el libro: " + response.StatusCode);
            }
        }

        private async void AddLibro()
        {
            ViewModels.LibrosViewModel oLibro = new ViewModels.LibrosViewModel();
            oLibro.ISBN = txtISBN.Text;
            oLibro.Titulo = txtTitulo.Text;
            oLibro.Autor = txtAutor.Text;
            oLibro.Temas = txtTemas.Text;
            oLibro.Editorial = txtEditorial.Text;
            using (var client = new HttpClient())
            {
                var serializedLibro = JsonConvert.SerializeObject(oLibro);
                var content = new StringContent(serializedLibro, Encoding.UTF8, "application/json");
                var result = await client.PostAsync("https://localhost:44343/api/Libros", content);
                if (result.IsSuccessStatusCode)
                    MessageBox.Show("Libro registrado correctamente");
                else
                    MessageBox.Show("Error al registrar el libro: " + result.Content.ReadAsStringAsync().Result);
            }
            Limpiar();
            GetAllLibros();
        }


        private async void UpdateLibro(int id)
        {
            ViewModels.LibrosViewModel oLibro = new ViewModels.LibrosViewModel();
            oLibro.Id = id;
            oLibro.ISBN = txtISBN.Text;
            oLibro.Autor = txtAutor.Text;
            oLibro.Titulo = txtTitulo.Text;
            oLibro.Temas = txtTemas.Text;
            oLibro.Editorial = txtEditorial.Text;

            using (var client = new HttpClient())
            {
                var serializedLibro = JsonConvert.SerializeObject(oLibro);
                var content = new StringContent(serializedLibro, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = await client.PutAsync("https://localhost:44343/api/Libros/" + oLibro.Id, content);
                if (responseMessage.IsSuccessStatusCode)
                    MessageBox.Show("Registro actualizado");
                else
                    MessageBox.Show("Error al actualizar el libro: " + responseMessage.StatusCode);
            }
            Limpiar();
            GetAllLibros();
        }

        private async void DeleteLibro(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44343/api/Libros");
                HttpResponseMessage response = await client.DeleteAsync(String.Format("{0}/{1}", "https://localhost:44343/api/Libros", id));
                if (response.IsSuccessStatusCode)
                    MessageBox.Show("Libro eliminado con éxito");
                else
                    MessageBox.Show("No se pudo eliminar el libro: " + response.StatusCode);
            }
            Limpiar();
            GetAllLibros();
        }


        private void Limpiar()
        {
            txtISBN.Text = String.Empty;
            txtAutor.Text = String.Empty;
            txtTitulo.Text = String.Empty;
            txtTemas.Text = String.Empty;
            txtEditorial.Text = String.Empty;
            id = 0;
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            AddLibro();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (id != 0)
                UpdateLibro(id);
        }


        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (id != 0)
                DeleteLibro(id);
        }

        private void dgvLibros_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            foreach (DataGridViewRow row in dgvLibros.Rows)
            {
                if (row.Index == e.RowIndex)
                {
                    id = int.Parse(row.Cells[0].Value.ToString());
                    GetLibroById(id);
                }
            }
        }
    }
}
