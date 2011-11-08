#region GNU license
// MyFilms - Plugin for Mediaportal
// http://www.team-mediaportal.com
// Copyright (C) 2006-2007
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
#endregion

namespace MyFilmsPlugin.MyFilms.Utils
{
  using System;
  using System.IO;
  using System.Text;
  using System.Security.Cryptography;

  public class Crypto
	{
		byte[] Clef = {0xAD, 0x24, 0xFE, 0x58, 0xC5, 0x81, 0x37, 0xB4, 0xF9, 0x97, 0x23, 0xD2, 0x13, 0x86, 0xBB, 0xA7};
		byte[] Vect = {0x81, 0xFD, 0xC3, 0xBB, 0x0A, 0xE6, 0xFE, 0xB8, 0xD9, 0xC0, 0x0C, 0x92, 0x73, 0xD4, 0x1A, 0xF2};
		
		RijndaelManaged rj = new RijndaelManaged();

		public Crypto()
		{
			// Constructeur : Code exécuté à chaque création d'un objet CryptageTexte.Crypto() : aucun !
			// Ce constructeur est nécessaire, même "vide".
		}

		
		// ************************ CRYPTER(Textebrut)*******************************
		/// <summary>
		/// Fonction de cryptage : elle necessite en argument une chaîne de caractères,
		/// et renvoie une chaîne de caractères cryptée (cipher-text).
		/// </summary>
		/// <param name=" TexteBrut"></param>
		/// <returns name="string CypherTexte"></returns>
		// ***************************************************************************

		public string Crypter(string TexteBrut)
		{
            if (TexteBrut.Length == 0)
              return string.Empty;
			MemoryStream CypherTexteMem = new MemoryStream();
			
			CryptoStream CStream = new CryptoStream(CypherTexteMem, 
			rj.CreateEncryptor(Clef, Vect),	CryptoStreamMode.Write);
			
			byte[] TextebrutByte = new UnicodeEncoding().GetBytes(TexteBrut);
            
			CStream.Write(TextebrutByte, 0, TextebrutByte.Length);
 			CStream.Close();
			
			byte[] CypherTexteByte = CypherTexteMem.ToArray();
			
			CypherTexteMem.Close();
			string CypherTexte = new UnicodeEncoding().GetString(CypherTexteByte);
			
			return CypherTexte;
		}


		// ************************ DECRYPTER(Textebrut)*****************************
		/// <summary>
		/// Fonction de décryptage : elle necessite en argument une chaîne de 
		/// caractères cryptés (cipher-text) et renvoie une chaîne de caractères.
		/// </summary>
		/// <param name="string CypherTexte"></param>
		/// <returns name="string Textebrut"></returns>
		// ***************************************************************************

		public string Decrypter(string CypherTexte)
		{
            if (CypherTexte.Length == 0)
              return string.Empty;	
			MemoryStream CypherTexteMem = new MemoryStream(new UnicodeEncoding().GetBytes(CypherTexte));
			
			CryptoStream CStream = new CryptoStream(CypherTexteMem, rj.CreateDecryptor(Clef, Vect),CryptoStreamMode.Read);
			
			MemoryStream TextebrutMem = new MemoryStream();
			
			do
			{
				byte[] buf = new byte[100];
				
				Int32 BytesLus = CStream.Read(buf,0,100);
				
				if (0 == BytesLus)
					break;
				
				TextebrutMem.Write(buf,0,BytesLus);

			}while(true);
			
			CStream.Close();
			CypherTexteMem.Close();
			
			byte[] TextebrutByte = TextebrutMem.ToArray();
			
			TextebrutMem.Close();
			
			string Textebrut = new UnicodeEncoding().GetString(TextebrutByte);
			return Textebrut;
		}
	}
}
