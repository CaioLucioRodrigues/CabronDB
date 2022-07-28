using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ecalc.CabronBase.Project
{
    public sealed class CabronDB
    {
        public CabronDB()
        {
            DefinirCaminho(@"C:\");
        }

        public string DiretorioBase { get; private set; }

        public string DataBaseName { get; private set; }

        public void DefinirCaminho(string caminho)
        {
            DiretorioBase = caminho;
        }

        public string CaminhoCompleto() => $@"{DiretorioBase}\{DataBaseName}";

        public void CreateDataBase(string dataBaseName)
        {
            Utils.CriarBancoDeDados(LocalizarBanco(banco: dataBaseName));
            DataBaseName = dataBaseName;
        }

        public void CreateTable(string nome, params string[] campos)
        {
            //Utils.ValidarSeTabelaExisteParaCreate(Localizar(nome));

            using (FileStream arquivo = File.Create(Localizar(nome)))
            {
                var titulos = Utils.MontarTitulos(campos);
                Utils.EscreverTitulos(arquivo, texto: titulos);
            }
        }

        public void Insert(string tabela, params string[] valores)
        {
            Utils.ValidarSeTabelaExisteParaSelect(Localizar(tabela));
            Utils.ValidarQuantidadeDeCampos(Localizar(tabela), valores);

            var texto = Utils.MontarValores(valores);
            Utils.EscreverValores(Localizar(tabela), texto);
        }

        public void Select(string tabela, params string[] campos)
        {
            Utils.ValidarSeTabelaExisteParaSelect(Localizar(tabela));

            var registros = Utils.FiltrarCampos(Localizar(tabela), campos);
            Utils.ImprimirRegistros(registros);
        }

        public void SelectWhere(string tabela, string condicao)
        {
            var registros = Utils.FiltrarRegistros(Localizar(tabela), condicao);
            Utils.ImprimirRegistros(registros);
        }

        private string Localizar(string tabela) => $@"{CaminhoCompleto()}\{tabela}.txt";

        private string LocalizarBanco(string banco) => $@"{DiretorioBase}\{banco}";
    }
}
