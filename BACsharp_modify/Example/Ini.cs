﻿/**************************************************************************
*
* THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
* EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
* OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
* IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
* CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
* TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
* SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*
*********************************************************************/

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BACsharp_modify.Example
{
  /// <summary>
  /// Create a New INI file to store or load data
  /// </summary>
  public class IniFile
  {
    public string path;

    [DllImport("kernel32")]
    private static extern long WritePrivateProfileString(string section,
      string key,string val,string filePath);
    [DllImport("kernel32")]
    private static extern int GetPrivateProfileString(string section,
      string key,string def, StringBuilder retVal,
      int size,string filePath);

    /// <summary>
    /// INIFile Constructor.
    /// </summary>
    /// <PARAM name="INIPath"></PARAM>
    public IniFile(string INIPath)
    {
      path = INIPath;
    }
    /// <summary>
    /// Write Data to the INI File
    /// </summary>
    /// <PARAM name="Section"></PARAM>
    /// Section name
    /// <PARAM name="Key"></PARAM>
    /// Key Name
    /// <PARAM name="Value"></PARAM>
    /// Value Name
    public void IniWriteValue(string Section,string Key,string Value)
    {
      WritePrivateProfileString(Section,Key,Value,this.path);
    }
        
    /// <summary>
    /// Read Data Value From the Ini File
    /// </summary>
    /// <PARAM name="Section"></PARAM>
    /// <PARAM name="Key"></PARAM>
    /// <PARAM name="Path"></PARAM>
    /// <returns></returns>
    public string IniReadValue(string Section,string Key)
    {
      StringBuilder temp = new StringBuilder(255);
      int i = GetPrivateProfileString(Section,Key,"",temp,255,this.path);
      return temp.ToString();
    }
  }
}
