using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ecalc.CabronBase.Project
{
    public static class Utils
    {
        public const int TAMANHO_CAMPO = 30;

        public static void CriarBancoDeDados(string banco)
        {
            Directory.CreateDirectory(banco);
        }

        public static string MontarTitulos(string[] campos)
        {
            return MontarTexto(campos);
        }

        public static string MontarValores(string[] valores)
        {
            return MontarTexto(valores);
        }

        private static string MontarTexto(string[] valores)
        {
            var texto = string.Empty;
            for (int i = 0; i <= valores.Length - 1; i++)
                texto += valores[i].PadRight(TAMANHO_CAMPO, ' ');
            return texto;
        }

        public static void EscreverTitulos(FileStream arquivo, string texto)
        {
            byte[] conteudo = new UTF8Encoding(true).GetBytes($"{texto}{Environment.NewLine}");
            arquivo.Write(conteudo, 0, conteudo.Length);
        }

        public static void EscreverValores(string tabela, string texto)
        {
            File.AppendAllText(tabela, $"{texto}{Environment.NewLine}");
        }

        public static void ImprimirRegistros(List<string> registros)
        {            
            System.Console.WriteLine(' ');
            System.Console.WriteLine(' ');
            var primeiraLinha = true;
            foreach (var registro in registros)
            {
                if (primeiraLinha)
                {
                    System.Console.WriteLine(new string('-', 120));
                    System.Console.WriteLine(registro);
                    System.Console.WriteLine(new string('-', 120));
                    primeiraLinha = false;
                }
                else
                    System.Console.WriteLine(registro);
            }
            System.Console.WriteLine(new string('-', 120));
        }

        public static void ValidarQuantidadeDeCampos(string tabela, string[] valoresASeremInseridos)
        {
            var titulos = ObterTitulos(tabela);
            var quantidadeDeCampos = ObterQuantidadeDeCampos(titulos);

            if (valoresASeremInseridos.Length != quantidadeDeCampos)
                throw new Exception($"SQL Error: Quantidade de campos incorreta. Tabela possui {quantidadeDeCampos} campos e o " +
                    $"script possui {valoresASeremInseridos.Length} campos.");
        }

        private static string ObterTitulos(string tabela)
        {
            return File.ReadLines(tabela).First();
        }

        private static int ObterQuantidadeDeCampos(string registro)
        {
            return ObterTodosOsCampos(registro).Count();
        }

        public static List<string> FiltrarCampos(string tabela, params string[] campos)
        {
            if (campos.Length == 0)
                throw new Exception("SQL Error: Sintaxe do comando Select incorreta! Indicar quais campos devem ser selecionados.");
                
            if (campos[0] == "*")
                return ObterRegistrosComTodosOsCampos(tabela);

            var camposFiltrados = new List<string>();
            var linha = 0;
            var posicoes = new List<int>();
            foreach (string registro in System.IO.File.ReadLines(tabela))
            {
                linha++;
                var texto = string.Empty;
                if (PrimeiraLinha(linha))
                {
                    var contadorCampos = 0;
                    var todosCampos = ObterTodosOsCampos(registro);
                    todosCampos.ForEach(campo =>
                    {
                        if (campos.Contains(campo))
                            posicoes.Add(contadorCampos);
                        contadorCampos++;
                    });                    
                }
                foreach (var posicao in posicoes)
                {
                    if (registro.Length >= posicao * Utils.TAMANHO_CAMPO + Utils.TAMANHO_CAMPO)
                        texto += registro.Substring(posicao * Utils.TAMANHO_CAMPO, Utils.TAMANHO_CAMPO);
                }
                camposFiltrados.Add(texto);
            }
            return camposFiltrados;
        }

        public static List<string> FiltrarRegistros(string tabela, string condicao)
        {
            var registrosFiltrados = new List<string>();

            var argumentos = condicao.Split(' ');
            if (argumentos.Length != 3)
                throw new Exception("SQL Error: Sintaxe da condição incorreta! Usar 'Campo Operador Valor");

            var campoCondicao = argumentos[0];
            var operadorCondicao = argumentos[1];
            var valorCondicao = argumentos[2];

            var colunas = ObterTitulos(tabela);
            registrosFiltrados.Add(colunas);
            if (!colunas.Contains(campoCondicao))
                throw new Exception($"SQL Error: Tabela '{tabela}' não possui o campo '{campoCondicao}'!");

            if ((operadorCondicao != "=") && (operadorCondicao != "like"))
                throw new Exception($"SQL Error: Operador '{operadorCondicao}' não suportado! Hoje os operadores suportado são '*' e 'like'");

            int posicaoCampoCondicao = ObterTodosOsCampos(colunas).FindIndex(coluna => coluna.Trim() == campoCondicao);            

            foreach (string registro in System.IO.File.ReadLines(tabela))
            {
                if (operadorCondicao == "=")
                {
                    if (registro.Substring(posicaoCampoCondicao * TAMANHO_CAMPO, TAMANHO_CAMPO).Trim() == valorCondicao)
                        registrosFiltrados.Add(registro);
                }
                else if (operadorCondicao == "like")
                {
                    if (registro.Substring(posicaoCampoCondicao * TAMANHO_CAMPO, TAMANHO_CAMPO).Trim().Contains(valorCondicao))
                        registrosFiltrados.Add(registro);
                }
            }

            return registrosFiltrados;
        }

        private static bool PrimeiraLinha(int linha) => linha == 1;

        private static List<string> ObterTodosOsCampos(string registro)
        {
            return new List<string>(registro.Split(' '))
                                            .Where(campo => !string.IsNullOrEmpty(campo))
                                            .ToList();
        }

        private static List<string> ObterRegistrosComTodosOsCampos(string tabela)
        {
            var registros = new List<string>();
            foreach (string registro in System.IO.File.ReadLines(tabela))
            {
                registros.Add(registro);
            }
            return registros;
        }

        public static void ValidarSeTabelaExisteParaSelect(string tabela)
        {
            if (!File.Exists(tabela))
                throw new Exception($"SQL Error: Tabela '{tabela}' não existe!");
        }

        public static void ValidarSeTabelaExisteParaCreate(string tabela)
        {
            if (File.Exists(tabela))
                throw new Exception($"SQL Error: Tabela '{tabela}' já existe!");
        }
    }
}
