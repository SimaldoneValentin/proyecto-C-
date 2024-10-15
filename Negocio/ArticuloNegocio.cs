using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dominio;
using Negocio;
using System.Collections;
using System.Xml.Linq;


namespace Negocio
{
    public class ArticuloNegocio
    {
        public List<Articulo> listar()
        {
            List<Articulo> lista = new List<Articulo>();
            SqlConnection conexion = new SqlConnection();
            SqlCommand comando = new SqlCommand();
            SqlDataReader lector;

            try
            {
                conexion.ConnectionString = "server=.\\SQLEXPRESS; database=CATALOGO_DB; integrated security=true";
                comando.CommandType = System.Data.CommandType.Text;
                comando.CommandText = "Select A.Id, Codigo, Nombre, A.Descripcion, ImagenUrl, Precio, C.Descripcion Categoria, M.Descripcion Marca, A.IdCategoria, A.IdMarca From articulos A, CATEGORIAS C, MARCAS M Where  C.Id = A.IdCategoria And M.Id = A.IdMarca";
                comando.Connection = conexion;

                conexion.Open();
                lector = comando.ExecuteReader();

                while (lector.Read())
                {

                    Articulo aux = new Articulo
                    {
                        Id = (int)lector["Id"],
                        Codigo = lector["Codigo"] != DBNull.Value ? (string)lector["Codigo"] : string.Empty,
                        Nombre = lector["Nombre"] != DBNull.Value ? (string)lector["Nombre"] : string.Empty,
                        Descripcion = lector["Descripcion"] != DBNull.Value ? (string)lector["Descripcion"] : string.Empty,
                        ImagenUrl = lector["ImagenUrl"] != DBNull.Value ? (string)lector["ImagenUrl"] : string.Empty,
                        Precio = lector["Precio"] != DBNull.Value ? (decimal)lector["Precio"] : 0,
                        Categoria = new Categoria
                        {
                            Id = (int)lector["IdCategoria"],
                            Descripcion = lector["Categoria"] != DBNull.Value ? (string)lector["Categoria"] : string.Empty
                        },
                        Marca = new Marca
                        {
                            Id = (int)lector["IdMarca"],
                            Descripcion = lector["Marca"] != DBNull.Value ? (string)lector["Marca"] : string.Empty
                        }
                    };

                    lista.Add(aux);
                }
                conexion.Close();
                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void agregar(Articulo nuevo)
        {
            Accesodatos datos = new Accesodatos();

            try
            {
                datos.setearconsulta("Insert into ARTICULOS  (Codigo, Nombre, Descripcion, ImagenUrl , IdCategoria, IdMarca, Precio)values(@Codigo, @Nombre, @Descripcion ,@ImagenUrl, @IdCategoria, @IdMarca, @Precio)");
                datos.setearParametro("@Codigo", nuevo.Codigo);
                datos.setearParametro("@Nombre", nuevo.Nombre);
                datos.setearParametro("@Descripcion", nuevo.Descripcion);
                datos.setearParametro("@ImagenUrl", nuevo.ImagenUrl);
                datos.setearParametro("@IdCategoria", nuevo.Categoria.Id);
                datos.setearParametro("@IdMarca", nuevo.Marca.Id);
                datos.setearParametro("@Precio", nuevo.Precio);


                datos.ejecuarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void modificar(Articulo artic)
        {
                Accesodatos datos = new Accesodatos();
                try
                {
                    datos.setearconsulta("update ARTICULOS set Codigo = @Codigo, Nombre = @Nombre, Descripcion = @Descripcion, ImagenUrl = @ImagenUrl, Precio = @Precio, IdCategoria = @IdCategoria, IdMarca = @IdMarca where id = @id");
                    datos.setearParametro("@Codigo", artic.Codigo);
                    datos.setearParametro("@Nombre", artic.Nombre);
                    datos.setearParametro("@Descripcion", artic.Descripcion);
                    datos.setearParametro("@ImagenUrl", artic.ImagenUrl);
                    datos.setearParametro("@Precio", artic.Precio);
                    datos.setearParametro("@IdCategoria", artic.Categoria.Id);
                    datos.setearParametro("@IdMarca", artic.Marca.Id);
                    datos.setearParametro("@Id", artic.Id);
        
                datos.ejecuarAccion();
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                finally
                {
                    datos.cerrarConexion();
                }
        }

        public List<Articulo> Filtrar(string campo, string criterio, string filtro)
        {
            List<Articulo> Lista = new List<Articulo>();
            Accesodatos datos = new Accesodatos();
            try
            {
                string consulta = "SELECT A.Id, Codigo, Nombre, A.Descripcion, ImagenUrl, Precio, C.Descripcion Categoria, M.Descripcion Marca, A.IdCategoria, A.IdMarca FROM ARTICULOS A, CATEGORIAS C, MARCAS M WHERE C.Id = A.IdCategoria AND M.Id = A.IdMarca AND ";

                if (campo == "Codigo")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "Codigo LIKE '" + filtro + "%'";
                            break;
                        case "Termina con":
                            consulta += "Codigo LIKE '%" + filtro + "'";
                            break;
                        case "Contiene":
                            consulta += "Codigo LIKE '%" + filtro + "%'";
                            break;
                    }
                }
                else if (campo == "Nombre")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "Nombre LIKE '" + filtro + "%'";
                            break;
                        case "Termina con":
                            consulta += "Nombre LIKE '%" + filtro + "'";
                            break;
                        case "Contiene":
                            consulta += "Nombre LIKE '%" + filtro + "%'";
                            break;
                    }
                }
                else if (campo == "Marca")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "M.Descripcion LIKE '" + filtro + "%'";
                            break;
                        case "Termina con":
                            consulta += "M.Descripcion LIKE '%" + filtro + "'";
                            break;
                        case "Contiene":
                            consulta += "M.Descripcion LIKE '%" + filtro + "%'";
                            break;
                    }
                }
                else if (campo == "Precio")
                {
                    switch (criterio)
                    {
                        case "Menor o igual a":
                            consulta += "Precio <= " + filtro;
                            break;
                        case "Mayor o igual a":
                            consulta += "Precio >= " + filtro;
                            break;
                        case "Igual a":
                            consulta += "Precio = " + filtro;
                            break;
                    }
                }
                else
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "Descripcion LIKE '" + filtro + "%'";
                            break;
                        case "Termina con":
                            consulta += "Descripcion LIKE '%" + filtro + "'";
                            break;
                        case "Contiene":
                            consulta += "Descripcion LIKE '%" + filtro + "%'";
                            break;
                    }
                }

                datos.setearconsulta(consulta);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Codigo = (string)datos.Lector["Codigo"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];
                    if (!(datos.Lector["ImagenUrl"] is DBNull))
                        aux.ImagenUrl = (string)datos.Lector["ImagenUrl"];
                    aux.Precio = datos.Lector["Precio"] != DBNull.Value ? (decimal)datos.Lector["Precio"] : 0;
                    aux.Categoria = new Categoria
                    {
                        Id = (int)datos.Lector["IdCategoria"],
                        Descripcion = (string)datos.Lector["Categoria"]
                    };
                    aux.Marca = new Marca
                    {
                        Id = (int)datos.Lector["IdMarca"],
                        Descripcion = (string)datos.Lector["Marca"]
                    };

                    Lista.Add(aux);
                }

                return Lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
        public void eliminar(int id)
        {
            try
            {
                Accesodatos datos = new Accesodatos();
                datos.setearconsulta("delete from articulos where id = @id");
                datos.setearParametro("@id", id);
                datos.ejecuarAccion();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
