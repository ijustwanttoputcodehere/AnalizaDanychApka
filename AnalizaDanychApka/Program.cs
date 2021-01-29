using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;
using Porter2StemmerStandard;

namespace AnalizaDanychApka
{
    class Program
    {


        static void Main(string[] args)
        {

            List<string> properPlaces = new List<string> { "usa", "west-germany", "france" , "uk","japan", "canada"};
            List<string> myPlaces = new List<string> { };
            List<string> myBody = new List<string> { };
            List<TestObj> Testowe = new List<TestObj> { };


            DirectoryInfo d = new DirectoryInfo(@"E:\Pulpit\1111111");//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.sgm"); //Getting Text files
            int o = 0;
            foreach (FileInfo file in Files)
            { 
            const Int32 BufferSize = 128;
            //string path = @"E:\Pulpit\1111111\reut2-001.sgm";
            using (var fileStream = File.OpenRead(file.FullName))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                //int i = 0;
                String line;
                String wynik = ""; ;

                line = streamReader.ReadLine();

                while ((line = streamReader.ReadLine()) != null)
                {
                    //Podmieniam znaki specjalne na takie ktore nie wchodza w konflikt z xml
                    wynik += ReplaceHexadecimalSymbols(line);

                    //if (line.Contains("<PLACES>"))
                    //{ }

                    //Console.WriteLine(line);


                    //i++;
                    //if (i > 20) 
                    //{ break; }


                }

                var Myreplacedxml = "<root>" + wynik + "</root>";
                //wynik += ("<root>" + wynik + "</root>");


                XmlSerializer Serializer = new XmlSerializer(typeof(root));
                root result;
                using (TextReader reader = new StringReader(Myreplacedxml))
                    result = (root)Serializer.Deserialize(reader);

                foreach (rootREUTERS rootREUTER in result.REUTERS)
                {
                    if (rootREUTER.PLACES.Length != 1)
                        continue;
                    else if (!properPlaces.Contains(rootREUTER.PLACES[0]))
                        continue;
                    else
                    {
                        if(!(rootREUTER.TEXT.BODY is null))
                        { 
                        myPlaces.Add(rootREUTER.PLACES[0]);
                        myBody.Add(rootREUTER.TEXT.BODY);

                        

                                //Console.WriteLine(rootREUTER.TEXT.BODY);
                        }

                            //Console.WriteLine(rootREUTER.PLACES[0]);
                    }
                }



                //Console.ReadKey();
                // Process line
            }
                //odczyt jenego pliku

                //o++;
                //if(o > 9) {
                //    break;
                    Console.WriteLine("Processing " + file.FullName + "...");
               // }
            }


           

            EnglishPorter2Stemmer stemmer = new EnglishPorter2Stemmer();
            
            List<int> feature2 = new List<int> { };


            //int i = 0;
            //while (myBody[i] != null) 

            //Tutaj testuje stemowanie i wybieranie + wpisywanie cech do listyw obiekcie
            List<string> allwords = new List<string> { };
            Dictionary<string, int> wordsDictionary = new Dictionary<string, int>();
           for (int i = 0 ; i < myBody.Count; i++)
           {

            char[] separator = { '.', ',', ' ', '\t', '"', '=', '-', '<', '>', ')', '(', ';' };
            string[] worlds = myBody[i].Split(separator);
            List<double> feature1 = new List<double> { };
                

                string pom;
            double world7 = 0;
            double worldS = 0;
            //int world5 = 0;
            

            foreach (string world in worlds) 
            {

               pom = stemmer.Stem(world).Value;
               
                if(!wordsDictionary.ContainsKey(pom) )
                    {
                       
                        wordsDictionary.Add(pom, 1);
                    }
                else
                    {
                        wordsDictionary[pom]++;
                    }
                    //Console.WriteLine(pom);

                    //Zliczam ilosc slow dluzszych powyzej 7 znakow
                
                

            }

           }
            foreach (KeyValuePair<string, int> kvp in wordsDictionary) 
            {

                    if (kvp.Value > 10) 
                    {
                        allwords.Add(kvp.Key);
                    }
                
            }  
                
                //double w1 = worldS;
                //double w2 = world7;
                //double normal = Math.Sqrt((w1*w1) + (w2*w2));                
                //world7 = world7 / normal;
                //worldS = worldS / normal;
                

               // feature1.Add(worldS);
               // feature1.Add(world7);
               // Testowe.Add(new TestObj(myPlaces[i], feature1));
               

            
            //Dodaje cechy i tworze obiekty
            for (int i = 0; i < myBody.Count; i++)
            {

                string[] worlds = myBody[i].Split(' ');

                List<double> feature1 = new List<double> { };


                string pom;
                for (int j = 0; j < worlds.Length; j++ ) {
                    pom = stemmer.Stem(worlds[j]).Value;
                    worlds[j] = pom;
                }

                foreach (string world in allwords)
                {

                   
                    //allwords przechowuje wystąpienia słowa, myBody.Count to liczba dokumentów
                    double counter = 0;
                   if( worlds.Contains(world))
                    {
                        foreach (string wrd in worlds) 
                        { 
                        if (wrd.Equals(world)) { counter++;  }
                        }
                        //tf idf basically, myBody.Count to liczba obiektow aworldsDictionary przechowuje wystapienia slow w calym zbiorze
                        double pomocy = (double)myBody.Count / (double)wordsDictionary[world];
                        counter = counter * Math.Log( pomocy );
                        feature1.Add(counter);
                    }
                    else 
                    { 
                        feature1.Add(0); 
                    }
                   



                }



                 Testowe.Add(new TestObj(myPlaces[i], feature1));


            }
            // Nie jest kolorowo
            // zapisuje do pliku formatu csv żeby potem przetworzyć w pythonie
            /*
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"E:\Pulpit\Reuters_reduce_usa.csv"))
            {
                string LineToWrite = "labels,";
                foreach (string line in allwords)
                {
                     
                    LineToWrite += (line + ",");



                }

                file.WriteLine(LineToWrite);
                int liczusa = 0;
                foreach (TestObj testObj in Testowe)
                {
                    if ((testObj.label.Equals("usa"))) { liczusa++; }
                    if ((liczusa == 10) || (!testObj.label.Equals("usa"))) 
                    { 
                                LineToWrite = (testObj.label + ",");
                        foreach(int feature in testObj.features)
                        {
                            LineToWrite += feature.ToString() + ",";
                        }
                        file.WriteLine(LineToWrite);
                        if(liczusa == 10) { liczusa = 0; }
                        
                    }
                }

            }
            Console.WriteLine("Zapisywanie ukończone");
            */

            // Moje piękne wywołanie algorytmu
            KNN(10,Testowe,5);

            double KNN (int k, List<TestObj> objs,int odsetek)
            {
                double accuracy = 0;
                List<string> Predictions = new List<string>{};
                List<TestObj> Tests = new List<TestObj> { };
                List<TestObj> Verification = new List<TestObj> { };
                //dzielę zbior na testowy i testowany 
                //TODO zrobic mozliwosc wyboru odsetka ele w zb testowym
                int DeleteUsa = 0;
                int cykle = 0;
                int poprawne = 0;
                int licz = 1;
                foreach (TestObj obj in objs)
                {
                    //Dwa ify do usuwania 9/10 usa
                    if (obj.label.Equals("usa")) { DeleteUsa++; }
                    if ( (DeleteUsa == 10) || (!obj.label.Equals("usa")) ) { 
                    
                    if (licz <= odsetek)
                    {
                        Verification.Add(obj);
                        licz++;
                    }
                    else if ((licz > odsetek) && (licz < 10)) 
                    {
                        Tests.Add(obj);
                        licz++;
                    } 
                    else 
                    {
                        Tests.Add(obj);
                        licz = 1; 
                    }
                        DeleteUsa = 0;
                    }

                    
                }
                Console.WriteLine( Tests.Count + " | " + Verification.Count);

                //Tu sie bedzie dzialo
                foreach (TestObj oTe in Tests) 
                {
                    List<double> Neighbours = new List<double> { };
                    List<string> Nlabels = new List<string> { };
                    
                    
                    double sqdistance;
                    int iterator = 0;
                    
                    foreach (TestObj oVe in Verification) 
                    {
                        double distance = 0;
                        int i = 0;                        
                        while (i < oVe.features.Count)
                        {
                            distance += ((oTe.features[i] - oVe.features[i])* (oTe.features[i] - oVe.features[i]));
                            i++;
                        }                       
                        sqdistance = Math.Sqrt(distance);
                        // Znajduje k nn
                        
                        if (iterator < k) 
                        { 
                            Neighbours.Add(sqdistance) ;
                            Nlabels.Add(oVe.label);
                            iterator++;
                        }
                        else 
                        {
                            int potato = 0;
                            while(iterator > potato)
                            {
                                if(Neighbours[potato] > sqdistance) 
                                {
                                    Neighbours[potato] = sqdistance;
                                    Nlabels[potato] = oVe.label;
                                }
                                potato++;
                            }
                        }

                    }


                    /*
                    //Pora podjac decyzje
                    List<string> Ulabels = new List<string> { };
                    int j = 0;
                    foreach (string label in Nlabels) 
                    {
                        //musi byc petla dla labels nad tym ifem
                        int i = 0;
                        if (Ulabels.Count == 0) 
                        {
                            Ulabels.Add(label);
                                j++;
                        }

                        while (j < Ulabels.Count) 
                        { 
                            if (!Ulabels[j].Equals(label))
                            {
                                    Ulabels[j] = label;
                                    
                            }
                            j++;
                        }

                    }
                    */
                    List<string> Ulabels = new List<string> { };
                    foreach (string place in properPlaces)
                    {
                        if (Nlabels.Contains(place))
                        {
                            Ulabels.Add(place);
                        }
                    }

                    List<string> truewinner = new List<string> { };
                    int x = 0;
                    int max = 0;
                    
                    while (x < Ulabels.Count)
                    {
                        int counter = 0;
                        int y = 0;
                        while (y < Nlabels.Count)
                        {
                            if (Ulabels[x].Equals(Nlabels[y]))
                            {
                                counter++;
                                
                            }
                            y++;
                        }
                        if(x == 0) 
                        {
                            max = counter;
                            truewinner.Add(Ulabels[x]);
                        }
                        else if(counter > max)
                        {
                            truewinner.Clear();
                            truewinner.Add(Ulabels[x]);

                        }
                        else if((counter == max))
                        {

                            truewinner.Add(Ulabels[x]);
                            
                        
                        }
                        


                       x++;
                    }

                   // Console.WriteLine("Kraj przewidziany : " + truewinner[0] + " Kraj faktyczny : " + oTe.label + "\n");
                   // foreach (string label in Nlabels)
                    //{
                    //    Console.WriteLine(label + " | ");
                    //}
                
                    if (oTe.label.Equals(truewinner[0])) 
                    {
                        poprawne++;

                        //if(!(oTe.label == "usa"))
                        //{
                            //Console.WriteLine("Kraj dobrze przewidziany : " + oTe.label);
                        //}
                    }

                    Predictions.Add(truewinner[0]);
                    cykle++;
                   // Console.WriteLine("Ilosc cykli: " + cykle);

                }




                accuracy = (double)poprawne / cykle;
                Console.WriteLine("Wynik : " + accuracy);  
                return accuracy;
            }













            Console.ReadKey();

            string ReplaceHexadecimalSymbols(string txt)
            {
                string r = "[\x00-\x08\x0B\x0C\x0E-\x1F\x26]";
                return Regex.Replace(txt, r, "", RegexOptions.Compiled);
            }



        }

    }
}
