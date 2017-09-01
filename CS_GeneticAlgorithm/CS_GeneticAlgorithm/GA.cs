using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpeedyCoding;

namespace CS_GeneticAlgorithm
{
    public class GA
    {
        List<double[]> Genes = new List<double[]>();

        public GeneInfo Run( int population, int geneLen ,int iter)
        {
            var input = RandGenes(population,geneLen);
            return Process(input, iter);
        }

        public GeneInfo Run( List<double[]> input , int iter)
        {
            return Process(input, iter);
        }

        public GeneInfo Process( List<double[]> input ,int iter)
        {
            int i = 0;
            while ( i < iter )
            {
                var infos = input.Select( x => ToGeneInfo(x) ).ToList();
                var selected = Selection(infos);
                var child = CreateChild(selected);
                var result = NeedStop(child);
                if ( result.Item2 ) return result.Item1;
                i++;
            }
            return null;
        }



        public List<double[]> RandGenes( int population, int geneLen )
        {
            Random rnd = new Random();

            return
            Enumerable.Range(0, population).Select(x =>
             Enumerable.Range(0, geneLen).Select(y =>
             rnd.NextDouble() > 0.5 ? 1.0 : 0.0)
             .ToArray<double>())
            .ToList();
        }

        Func<double[], GeneInfo> ToGeneInfo =>
            gene =>
                GeneInfo.SetGene(gene,CalcFitness(gene));
            

        Func<List<GeneInfo>, List<GeneInfo>> Selection =>
            genelist =>
                genelist.OrderBy(x => x.Score).Take(2).ToList();


        Func<List<GeneInfo>, List<GeneInfo>> CreateChild =>
            selectedList =>
            {
                var pos = Create2Point( selectedList[0].Gene.Len() );

                //var corssed = Enumerable.Range( 0 , selectedList.Count() /2 )
                //    .Select( x => CrossOver( 
                //                        selectedList[x*2].Gene, 
                //                        selectedList[x * 2+1].Gene , 
                //                        pos) )
                //    .ToList();

                var coressed1 = Enumerable.Range( 0 , selectedList.Count() /2 )
                    .SelectMany(  f   =>  CrossOver(
                                        selectedList[f*2].Gene,
                                        selectedList[f * 2+1].Gene ,
                                        pos) ,
                                 (f,s) => Mutation(s) )
                    .ToList();

                // CrossOver and Mutation Algorithm

                return coressed1.Select(x => 
                                    GeneInfo.SetGene(x, CalcFitness(x)))
                                .ToList();
            };

        Func<List<GeneInfo>, Tuple<GeneInfo, bool>> NeedStop =>
            genelist =>
            {
                var best =  genelist.OrderBy(x => x.Score)
                             .First();
                if ( best.Score > 0.9 ) return Tuple.Create(best, true);
                else return Tuple.Create(best, false);
            };

        #region subLevel1 

        Func<double[], double> CalcFitness =>
            gene =>
            {
                return 0;
            };

        Func<int, int[]> Create2Point =>
            len =>
            {
                var val1 =(int)(new Random().Next(0,len));
                var val2 =(int)(new Random().Next(0,len));

                while ( val1 == val2 ) val2 = (int)( new Random().Next(0, len) );

                return new int[] { val1, val2 }.OrderBy(x => x).ToArray<int>();
            };

        Func<double[], double[], int[], List<double[]>> CrossOver =>
            ( fgene, sgene, pos ) =>
            {
                if ( fgene.Len() != sgene.Len() ) return null;

                int thr = fgene.Len() - pos[0] - pos[1];

                var c1 = fgene.Take(pos[0])
                              .Concat( sgene.Skip(pos[0]).Take(pos[1]) )
                              .Concat( fgene.Skip(pos.Sum()).Take(thr) )
                              .ToArray<double>();

                var c2 = sgene.Take(pos[0])
                              .Concat( fgene.Skip(pos[0]).Take(pos[1]) )
                              .Concat( sgene.Skip(pos.Sum()).Take(thr) )
                              .ToArray<double>();

                return new List<double[]>()
                                .Append(c1)
                                .Append(c2);
            };

        Func<double[], double[]> Mutation =>
            gene =>
            {
                var rnd = new Random();
                if ( rnd.NextDouble() > 0.5 )
                {
                    var pos = rnd.Next(0,gene.Len());
                    gene[pos] = gene[pos] == 1 ? 0 : 1;
                }
                return gene;
            };
        #endregion





    }

    public class GeneInfo
    {
        public double[] Gene;
        public double Score;

        private GeneInfo( double[] gene, double score )
        {
            Gene = gene;
            Score = score;
        }

        public static GeneInfo SetGene( double[] gene, double score )
        {
            return new GeneInfo(gene, score);
        }

    }

}
