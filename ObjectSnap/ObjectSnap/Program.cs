using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static alglib;
using Collada141;

namespace ObjectSnap
{
    class Program
    {
        static double[,] ConvertMatrix(double[] flat, int m, int n)
        {
            if (flat.Length != m * n)
            {
                throw new ArgumentException("Invalid length");
            }
            double[,] ret = new double[m, n];
            // BlockCopy uses byte lengths: a double is 8 bytes
            Buffer.BlockCopy(flat, 0, ret, 0, flat.Length * sizeof(double));
            return ret;
        }

        static void Main(string[] args)
        {
            //Console.WriteLine(alglib.svd.rmatrixsvd();
            COLLADA model = COLLADA.Load("C:/Users/mschommer/Documents/Github/Assembly-Designer/TestObjects/cube.dae");
            // Iterate on libraries
            

            foreach (var item in model.Items)
            {
                var geometries = item as library_geometries;
                if (geometries == null)
                    continue;
                //Console.WriteLine(geometries.geometry[0].Item);
                // Iterate on geomerty in library_geometries 
                foreach (var geom in geometries.geometry)
                {
                    var mesh = geom.Item as mesh;
                    double[,] sourceMatrix = {};
                    if (mesh == null)
                        continue;
                    
                    // Dump source[] for geom
                    foreach (var source in mesh.source)
                    {
                        var float_array = source.Item as float_array;
                        if (float_array == null)
                            continue;
                        
                        //Console.Write("Geometry {0} source {1} : ", geom.id, source.id);
                        sourceMatrix = ConvertMatrix(float_array.Values, 3, float_array.Values.Length/3);
                        foreach (var mesh_source_value in float_array.Values)
                            Console.Write("{0} ", mesh_source_value);
                        Console.WriteLine();
                    }

                    // Dump Items[] for geom
                    foreach (var meshItem in mesh.Items)
                    {

                        if (meshItem is vertices)
                        {
                            var vertices = meshItem as vertices;
                            var inputs = vertices.input;
                            foreach (var input in inputs)
                                Console.WriteLine("\t Semantic {0} Source {1}", input.semantic, input.source);
                        }
                        else if (meshItem is triangles)
                        {
                            var triangles = meshItem as triangles;
                            var inputs = triangles.input;
                            foreach (var input in inputs)
                                Console.WriteLine("\t Semantic {0} Source {1} Offset {2}", input.semantic, input.source, input.offset);
                            Console.WriteLine("\t Indices {0}", triangles.p);
                        }
                    }

                    alglib.rmatrixsvd(sourceMatrix, 3, sourceMatrix.GetLength(1), 1, 1, 2, *)
                    for (int i = 0; i < sourceMatrix.GetLength(1); i++)
                    {
                        Console.Write("[");
                        for (int j = 0; j < 3; j++)
                        {
                            Console.Write(" {0} ", sourceMatrix[j, i]);
                        }
                        Console.WriteLine("]");
                    }
                }
            }
            Console.ReadKey();

        }
    }
}
