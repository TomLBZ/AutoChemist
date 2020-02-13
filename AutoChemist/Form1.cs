using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace AutoChemist
{
    public partial class Chemist : Form
    {
        private readonly string[] StrElements = { "Text", "H", "He",
            "Li", "Be", "B", "C", "N", "O", "F", "Ne",
            "Na", "Mg", "Al", "Si", "P", "S", "Cl", "Ar",
            "K", "Ca", "Sc", "Ti", "V", "Cr", "Mn", "Fe", "Co", "Ni", "Cu", "Zn", "Ga", "Ge", "As", "Se", "Br", "Kr",
            "Rb", "Sr", "Y", "Zr", "Nb", "Mo", "Tc", "Ru", "Rh", "Pd", "Ag", "Cd", "In", "Sn", "Sb", "Te", "I","Xe",
            "Cs", "Ba", "La", "Ce", "Pr", "Nd", "Pm", "Sm", "Eu", "Gd", "Tb", "Dy", "Ho", "Er", "Tm", "Yb", "Lu","Hf","Ta", "W", "Re", "Os", "Ir", "Pt", "Au", "Hg", "Tl", "Pb", "Bi", "Po", "At", "Rn",
            "Fr", "Ra", "Ac", "Th", "Pa", "U", "Np", "Pu", "Am", "Cm", "Bk", "Cf", "Es", "Fm", "Md", "No", "Lr","Rf","Db", "Sg", "Bh", "Hs", "Mt", "Ds", "Rg", "Cn", "Nh", "Fl", "Mc", "Lv", "Ts", "Og",
            "DiyA", "DiyB", "DiyC","DiyD","DiyE", "DiyF", "DiyG","DiyH","DiyI", "DiyJ"};
        public Chemist()
        {
            InitializeComponent();
        }
        private ElementLable[] ELabels = new ElementLable[129];
        private void Chemist_Load(object sender, EventArgs e)
        {
            int segmentedlength = ClientSize.Width / 18;
            int interval = segmentedlength / 10;
            if (interval < 2)
            {
                interval = 2;
            }
            for (int i = 0; i < ELabels.Length; i++)
            {
                ELabels[i] = new ElementLable(i, segmentedlength - interval, interval);
                ELabels[i].ReferenceLocation = new Point(0, 0);
                ELabels[i].Refresh();
            }
            Controls.AddRange(ELabels);
        }
        private void Chemist_Resize(object sender, EventArgs e)
        {
            int segmentedlength = ClientSize.Width / 18;
            int interval = segmentedlength / 10;
            if (interval < 2)
            {
                interval = 2;
            }
            RefreshControls(segmentedlength - interval, interval);
        }
        private void Chemist_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
        private void RefreshControls(int width, int interval)
        {
            foreach (Control ctrl in Controls)
            {
                if (ctrl.GetType() == typeof(ElementLable))
                {
                    ElementLable el = (ElementLable)ctrl;
                    el.Width = width;
                    el.Height = width;
                    el.RenderedInterval = interval;
                    el.Refresh();
                }
            }
        }
        private void SetElementsToZero()
        {
            foreach (Control ctrl in Controls)
            {
                if (ctrl.GetType() == typeof(ElementLable))
                {
                    ElementLable el = (ElementLable)ctrl;
                    el.SetToZero();
                }
            }
        }
        private void btnCalc_Click(object sender, EventArgs e)
        {
            SetElementsToZero();
            txtMaterials.Text = txtMaterials.Text.Trim().Trim(',');
            int[][] materials = Elements(',');
            VisualizeMaterial(materials);
            txtResult.Text = GetVerboseAnswer(EvaluateMatrix(RREF(ReorganizeMaterials(materials, ElementDimension(materials)))));
        }
        private string GetVerboseAnswer(int[] result)
        {
            string output = "Result is: ";
            string reactants = "";
            string products = "";
            int i = 0;
            foreach (string str in txtMaterials.Text.Split(','))
            {
                int res = result[i++];
                if (res > 0)
                {
                    reactants += res.ToString() + str + " + ";
                }
                else if (res < 0)
                {
                    products += (-res).ToString() + str + " + ";
                }
            }
            reactants = reactants.Remove(reactants.Length - 2, 2);
            products = products.Remove(products.Length - 2, 2);
            output += reactants + " == " + products;
            return output;
        }
        private void btnLogChemical_Click(object sender, EventArgs e)
        {
            string text = ",";
            foreach (Control ctrl in Controls)
            {
                if (ctrl.GetType() == typeof(ElementLable))
                {
                    ElementLable el = (ElementLable)ctrl;
                    int weight = el.RenderMode > 0 ? el.RenderMode : -el.RenderMode;
                    if (el.RenderMode != 0)
                    {
                        text += el.ElementName;
                        if (weight > 1) { text += weight.ToString(); }
                    }
                    el.SetToZero();
                }
            }
            txtMaterials.Text += text;
        }
        private int[] EvaluateMatrix(int[][] rref) //output[index]=repetition
        {
            int rows = rref.Length;
            int cols = rref[0].Length;
            int[] output = new int[cols];
            int[] colMarker = new int[cols];
            int multifactor = 1;
            int[] leadingposition = new int[rows];
            for (int i = 0; i < rows; i++)
            {
                leadingposition[i] = cols - LeftMostValue(rref[i]);
                if (leadingposition[i] == cols)
                {
                    rows--;
                    Array.Resize(ref leadingposition, leadingposition.Length - 1);
                }
            }
            for (int i = 0; i < cols; i++)
            {
                if (!leadingposition.Contains(i))
                {
                    colMarker[i] = 1;
                    output[i] = 1;
                }
            }
            for (int i = rows - 1; i >= 0; i--)
            {
                int lp = leadingposition[i];
                int constant = 0;
                for (int j = lp + 1; j < cols; j++)
                {
                    constant -= colMarker[j] * rref[i][j];
                }
                int gcd = GreatestCommonDivider(constant, rref[i][lp]);
                output[lp] = constant / gcd;
                multifactor = rref[i][lp] / gcd;
                for (int j = lp + 1; j < cols; j++)
                {
                    colMarker[j] *= multifactor;
                    output[j] *= multifactor;
                }
                colMarker[lp] = output[lp];
            }
            output = Reduce(output);
            return output;
        }
        private void VisualizeMatrix(int[][] matrix)
        {
            string str = "";
            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    str += matrix[i][j].ToString() + "\t";
                }
                str += "\n";
            }
            MessageBox.Show(str);
        }
        private void VisualizeArray(int[] array)
        {
            string str = "";
            for (int i = 0; i < array.Length; i++)
            {
                str += array[i].ToString() + "\t";
            }
            MessageBox.Show(str);
        }
        private int[][] RREF(int[][] materials)
        {
            materials.OrderBy(o => LeftMostValue(o));
            int rows = materials.Length;
            int cols = materials[0].Length;
            int currentrow = 0;
            for (int c = 0; c < cols; c++)
            {
                int nonZeroRow = -1;
                for (int r = currentrow; r < rows; r++)
                {
                    if (materials[r][c] != 0)
                    {
                        nonZeroRow = r;
                        break;
                    }
                }
                if (nonZeroRow == -1) { continue; } //no non-zero rows, stop.
                int[] tmprow = new int[cols];
                Array.Copy(materials[currentrow], 0, tmprow, 0, cols);
                Array.Copy(materials[nonZeroRow], 0, materials[currentrow], 0, cols);
                Array.Copy(tmprow, 0, materials[nonZeroRow], 0, cols);
                for (int r = 0; r < rows; r++)
                {
                    if (r == currentrow) { continue; } //skip current row.
                    if (materials[r][c] != 0)
                    {
                        int gcd = GreatestCommonDivider(materials[r][c], materials[currentrow][c]);
                        int localMultiplier = materials[currentrow][c] / gcd;
                        int currentRowMultiplier = materials[r][c] / gcd;
                        for (int i = 0; i < cols; i++)
                        {
                            materials[currentrow][i] *= currentRowMultiplier;
                            materials[r][i] *= localMultiplier;
                            materials[r][i] -= materials[currentrow][i];
                        }
                    }
                }
                currentrow++;
            }
            for (int c = 0; c < rows; c++)
            {
                materials[c] = Reduce(materials[c]);
            }
            return materials;
        }
        private int[] Reduce(int[] row)
        {
            int gcd = row[0];
            for (int i = 0; i < row.Length; i++)
            {
                gcd = GreatestCommonDivider(gcd, row[i]);
            }
            for (int i = 0; i < row.Length; i++)
            {
                if (gcd != 0) {row[i] /= gcd;}
            }
            return row;
        }
        private int GreatestCommonDivider(int a, int b)
        {
            if (a == 0) { return b; }
            return GreatestCommonDivider(b % a, a);
        }
        private int LeftMostValue(int[] row)
        {
            int output = row.Length;
            for (int i = 0; i < row.Length; i++)
            {
                if (row[i] == 0)
                {
                    output--;
                }
                else
                {
                    break;
                }
            }
            return output;
        }
        private int[][] ReorganizeMaterials(int[][] materials, int[] elements)
        {
            int row = elements.Length;
            int col = materials.Length;
            int[][] result = new int[row][];
            for (int i = 0; i < row; i++)
            {
                result[i] = new int[col];
            }
            for (int i = 0; i < col; i++)
            {
                int[] currentcol = new int[row];
                for (int j = 0; j < materials[i].Length; j++)
                {
                    int find = Array.IndexOf(elements, materials[i][j]);
                    if (find != -1)
                    {
                        currentcol[find]++;
                    }
                }
                for (int j = 0; j < row; j++)
                {
                    result[j][i] = currentcol[j];
                }
            }
            return result;
        }
        private int[] ElementDimension(int[][] materials)
        {
            List<int> result = new List<int>(0);
            for (int i = 0; i < materials.Length; i++)
            {
                for (int j = 0; j < materials[i].Length; j++)
                {
                    if (!result.Contains(materials[i][j]))
                    {
                        result.Add(materials[i][j]);
                    }
                }
            }
            return result.ToArray();
        }
        private void VisualizeMaterial(int[][] materials)
        {
            for (int i = 0; i < materials.Length; i++)
            {
                for (int j = 0; j < materials[i].Length; j++)
                {
                    ElementLable label = (ElementLable)Controls.Find("EL" + materials[i][j].ToString(), false)[0];
                    label.OnClick(label, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                }
            }
        }
        private int[][] Elements(char split)
        {
            List<int[]> result = new List<int[]>(0);
            string[] StrMaterials = txtMaterials.Text.Split(split);
            foreach (string str in StrMaterials)
            {
                string trimmed = str.Trim();
                result.Add(Decomposition(trimmed));
            }
            return result.ToArray();
        }
        private int[] GetComposition(string str)
        {
            List<int> result = new List<int>(0);
            char[] chars = str.ToCharArray();
            string tmp = "";
            for (int i = 0; i < chars.Length; i++)
            {
                tmp += chars[i].ToString();
                int trialindex = Array.IndexOf(StrElements, tmp);
                if (trialindex != -1)
                {
                    result.Add(trialindex);
                    tmp = "";
                }
                if ('2' <= chars[i] && chars[i] <= '9')
                {
                    int value = (int)char.GetNumericValue(chars[i]);
                    for (int j = 1; j < value; j++)
                    {
                        result.Add(result.Last());
                    }
                    tmp = "";
                }
            }
            return result.ToArray();
        }
        private int[] Decomposition(string str)
        {
            if (str == "") { throw new Exception("Empty Compound!"); }
            List<int> result = new List<int>(0);
            char[] chars = str.ToCharArray();
            int opens = 0;
            int closes = 0;
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '(') 
                { 
                    opens++;
                }
                else if (chars[i] == ')') 
                { 
                    closes++;
                }
            }
            if (opens != closes) { throw new Exception("Parentheses are not balanced!"); }
            int[] openindex = new int[opens];
            int[] closeindex = new int[closes];
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '(')
                {
                    openindex[opens - 1] = i;
                    opens--;
                }
                else if (chars[i] == ')')
                {
                    closeindex[closeindex.Length - closes] = i;
                    int cil = closeindex[closeindex.Length - closes];
                    closes--;
                    int oil = openindex[opens];
                    opens++;
                    if (oil != -1 && oil < cil - 1)
                    {
                        string target = new string(chars, oil + 1, cil - oil - 1);
                        int[] innerresult = Decomposition(target);
                        target = "(" + target + ")";
                        string blank = new string(' ', target.Length);
                        string numregex = "([0-9]*)";
                        string back = cil == chars.Length - 1 ? "" : new string(chars, cil + 1, chars.Length - cil - 1);
                        if (back != "" && Regex.IsMatch(back, numregex))
                        {
                            string matchednum = Regex.Matches(back, numregex)[0].Groups[1].Value;
                            target += matchednum;
                            int multiplier = matchednum == "" ? 1 : Int32.Parse(matchednum);
                            blank += matchednum == "" ? "" : new string(' ', matchednum.Length);
                            back = back.TrimStart(matchednum.ToCharArray());
                            for (int j = 1; j < multiplier; j++)
                            {
                                result.AddRange(innerresult);
                            }
                        }
                        result.AddRange(innerresult);
                        result.Sort();
                        str = str.Replace(target, blank);
                        chars = str.ToCharArray();
                    }
                }
            }
            str = str.Replace(" ", "");
            if (str == "") { return result.ToArray(); }
            string elementRegex = "([A-Z][a-z]*)([0-9]*)";
            string validateRegex = "^(" + elementRegex + ")+$";
            if (!Regex.IsMatch(str, validateRegex)) { throw new FormatException("Input string was in an incorrect format."); }
            foreach (Match match in Regex.Matches(str, elementRegex))
            {
                string name = match.Groups[1].Value;
                int count = match.Groups[2].Value != "" ? int.Parse(match.Groups[2].Value) : 1;
                int index = Array.IndexOf(StrElements, name);
                if (index != -1) { result.Add(index); }
                for (int i = 1; i < count; i++)
                {
                    result.Add(result.Last());
                }
            }
            return result.ToArray();
        }
    }

    public class ElementLable : Label
    {
        struct TypeGroup
        {
            public int Code;
            public string Name;
            public int[] List;
        }
        public int ElementIndex { get; set; }
        public int RenderedInterval { get; set; }
        public Point ReferenceLocation { get; set; }
        public string ElementName { get { return StrElements[ElementIndex]; } }
        public string ElementType { get { return GetType(ElementIndex); } }
        private readonly string[] StrElements = { "Text", "H", "He",
            "Li", "Be", "B", "C", "N", "O", "F", "Ne",
            "Na", "Mg", "Al", "Si", "P", "S", "Cl", "Ar",
            "K", "Ca", "Sc", "Ti", "V", "Cr", "Mn", "Fe", "Co", "Ni", "Cu", "Zn", "Ga", "Ge", "As", "Se", "Br", "Kr",
            "Rb", "Sr", "Y", "Zr", "Nb", "Mo", "Tc", "Ru", "Rh", "Pd", "Ag", "Cd", "In", "Sn", "Sb", "Te", "I","Xe",
            "Cs", "Ba", "La", "Ce", "Pr", "Nd", "Pm", "Sm", "Eu", "Gd", "Tb", "Dy", "Ho", "Er", "Tm", "Yb", "Lu","Hf","Ta", "W", "Re", "Os", "Ir", "Pt", "Au", "Hg", "Tl", "Pb", "Bi", "Po", "At", "Rn",
            "Fr", "Ra", "Ac", "Th", "Pa", "U", "Np", "Pu", "Am", "Cm", "Bk", "Cf", "Es", "Fm", "Md", "No", "Lr","Rf","Db", "Sg", "Bh", "Hs", "Mt", "Ds", "Rg", "Cn", "Nh", "Fl", "Mc", "Lv", "Ts", "Og",
            "DiyA", "DiyB", "DiyC","DiyD","DiyE", "DiyF", "DiyG","DiyH","DiyI", "DiyJ"};
        private readonly TypeGroup[] TypeGroups = new TypeGroup[] {
            new TypeGroup { Code = 0, Name = "Non-Metals", List = new int[] { 1, 6, 7, 8, 9, 15, 16, 17, 34, 35, 53 } },
            new TypeGroup { Code = 1, Name = "Alkaline Metals", List = new int[] { 3, 11, 19, 37, 55, 87 } },
            new TypeGroup { Code = 2, Name = "Alkaline Dirt Metals", List = new int[] { 4, 12, 20, 38, 56, 88 } },
            new TypeGroup { Code = 3, Name = "Transition Metals", List = new int[] { 21, 22, 23, 24, 25, 26, 27, 28, 29, 39, 40, 41, 42, 43, 44, 45, 46, 47, 72, 73, 74, 75, 76, 77, 78, 79, 104, 105, 106, 107, 108 } },
            new TypeGroup { Code = 4, Name = "Lanthanum Metals", List = new int[] { 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71 } },
            new TypeGroup { Code = 5, Name = "Actinium Metals", List = new int[] { 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103 } },
            new TypeGroup { Code = 6, Name = "Semi-conductors", List = new int[] { 5, 14, 32, 33, 51, 52, 85 } },
            new TypeGroup { Code = 7, Name = "Rare Gas", List = new int[] { 2, 10, 18, 36, 54, 86 } },
            new TypeGroup { Code = 8, Name = "Main Group Metals", List = new int[] { 13, 30, 31, 48, 49, 50, 80, 81, 82, 83, 84, 112 } },
            new TypeGroup { Code = 9, Name = "Unidentified", List = new int[] { 0, 109, 110, 111, 113, 114, 115, 116, 117, 118 } },
            new TypeGroup { Code = 10, Name = "DIY Elements", List = new int[] { 119, 120, 121, 122, 123, 124, 125, 126, 127, 128} } };
        private readonly Color[] MyColors = { Color.LightYellow, Color.MistyRose, Color.LightCoral, Color.LightPink, 
            Color.Violet, Color.BlueViolet, Color.SandyBrown, Color.SkyBlue, Color.Gray, Color.LightGray, Color.White };
        public int RenderMode = 0;
        public ElementLable(int index = 0, int width = 10, int renderedinterval = 2)
        {
            Name = "EL" + index.ToString();
            ElementIndex = index;
            Width = width;
            Height = Width;
            RenderedInterval = renderedinterval;
            ReferenceLocation = new Point(0, 0);
            Text = index.ToString() + "\n" + ElementName + "\n" + RenderMode.ToString();
            Click += new EventHandler(OnClick);
            BorderStyle = BorderStyle.FixedSingle;
            BackColor = MyColors[GetTypeCode(index)];
        }
        public override void ResetText()
        {
            Text = ElementIndex.ToString() + "\n" + ElementName + "\n" + RenderMode.ToString();
            Refresh();
        }
        public void OnClick(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            if (me.Button == MouseButtons.Left)
            {
                RenderMode++;
            }
            else
            {
                RenderMode--;
            }
            ResetText();
        }
        public void SetToZero()
        {
            RenderMode = 0;
            ResetText();
        }
        private string GetType(int index=0)
        {
            foreach (TypeGroup tg in TypeGroups)
            {
                if (tg.List.Contains(index))
                {
                    return tg.Name;
                }
            }
            return "Not Found";
        }
        private int GetTypeCode(int index = 0)
        {
            foreach (TypeGroup tg in TypeGroups)
            {
                if (tg.List.Contains(index))
                {
                    return tg.Code;
                }
            }
            return 0;
        }
        public override void Refresh()
        {
            Locate(ElementIndex);
            base.Refresh();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (RenderMode > 0)
            {
                ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle,
                      Color.Blue, RenderedInterval, ButtonBorderStyle.Solid,
                      Color.Blue, RenderedInterval, ButtonBorderStyle.Solid,
                      Color.Blue, RenderedInterval, ButtonBorderStyle.Solid,
                      Color.Blue, RenderedInterval, ButtonBorderStyle.Solid);
            }
            else if (RenderMode < 0)
            {
                ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle,
                      Color.Green, RenderedInterval, ButtonBorderStyle.Solid,
                      Color.Green, RenderedInterval, ButtonBorderStyle.Solid,
                      Color.Green, RenderedInterval, ButtonBorderStyle.Solid,
                      Color.Green, RenderedInterval, ButtonBorderStyle.Solid);
            }
            else
            {
                ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle, BackColor, ButtonBorderStyle.None);
            }
            e.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), Width / 5.0f, 0.0f);
        }
        private void Locate(int index = 0)
        {
            if (index == 0)
            {
                Left = ReferenceLocation.X;
                Top = ReferenceLocation.Y + 8 * (Width + RenderedInterval);
            }
            else if (index == 1)
            {
                Left = ReferenceLocation.X;
                Top = ReferenceLocation.Y;
            }
            else if (index == 2)
            {
                Left = ReferenceLocation.X + 17 * (Width + RenderedInterval);
                Top = ReferenceLocation.Y;
            }
            else if (index == 3 || index == 4 || index == 11 || index == 12)
            {
                Left = ReferenceLocation.X + (index % 8 - 3) * (Width + RenderedInterval);
                Top = ReferenceLocation.Y + (index / 8 + 1) * (Width + RenderedInterval);
            }
            else if (index > 4 && index < 11)
            {
                Left = ReferenceLocation.X + ((index - 3) % 8 + 10) * (Width + RenderedInterval);
                Top = ReferenceLocation.Y + Width + RenderedInterval;
            }
            else if (index > 12 && index < 19)
            {
                Left = ReferenceLocation.X + ((index - 11) % 8 + 10) * (Width + RenderedInterval);
                Top = ReferenceLocation.Y + 2 * (Width + RenderedInterval);
            }
            else if (index > 18 && index < 37)
            {
                Left = ReferenceLocation.X + (index - 19) % 18 * (Width + RenderedInterval);
                Top = ReferenceLocation.Y + 3 * (Width + RenderedInterval);
            }
            else if (index > 36 && index < 55)
            {
                Left = ReferenceLocation.X + (index - 37) % 18 * (Width + RenderedInterval);
                Top = ReferenceLocation.Y + 4 * (Width + RenderedInterval);
            }
            else if (index == 55 || index == 56 || index == 57 || index == 87 || index == 88 || index == 89)
            {
                Left = ReferenceLocation.X + (index - 55) % 32 * (Width + RenderedInterval);
                Top = ReferenceLocation.Y + ((index - 55) / 32 + 5) * (Width + RenderedInterval);
            }
            else if (index > 71 && index < 87)
            {
                Left = ReferenceLocation.X + ((index - 55) % 32 - 14) * (Width + RenderedInterval);
                Top = ReferenceLocation.Y + 5 * (Width + RenderedInterval);
            }
            else if (index > 103 && index < 119)
            {
                Left = ReferenceLocation.X + ((index - 87) % 32 - 14) * (Width + RenderedInterval);
                Top = ReferenceLocation.Y + 6 * (Width + RenderedInterval);
            }
            else if (index > 57 && index < 72)
            {
                Left = ReferenceLocation.X + (index - 55) % 32 * (Width + RenderedInterval);
                Top = ReferenceLocation.Y + 7 * (Width + RenderedInterval);
            }
            else if (index > 89 && index < 104)
            {
                Left = ReferenceLocation.X + (index - 87) % 32 * (Width + RenderedInterval);
                Top = ReferenceLocation.Y + 8 * (Width + RenderedInterval);
            }
            else
            {
                Left = ReferenceLocation.X + (index - 119 + 2) * (Width + RenderedInterval);
                Top = ReferenceLocation.Y;
            }
        }
    }
}
