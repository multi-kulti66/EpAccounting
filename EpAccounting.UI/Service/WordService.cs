// ///////////////////////////////////
// File: WordService.cs
// Last Change: 25.10.2017  22:40
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.Service
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.ViewModel;
    using Microsoft.Office.Interop.Word;
    using Application = Microsoft.Office.Interop.Word.Application;



    public class WordService : IWordService
    {
        #region Fields

        private Application wordApp;
        private Document wordDoc;

        private object oMissing = Missing.Value;

        #endregion



        #region IWordService Members

        public void CreateWordBill(BillItemEditViewModel billItemEditViewModel, bool visible)
        {
            if (File.Exists(Settings.Default.WordTemplateFilePath) == false)
            {
                throw new Exception(Resources.Exception_Message_NoPathToBillTemplate);
            }

            if (Directory.Exists(Settings.Default.BillFolderPath) == false)
            {
                throw new Exception(Resources.Exception_Message_NoPathToBillFolder);
            }

            this.wordApp = new Application();
            this.wordApp.DisplayAlerts = WdAlertLevel.wdAlertsNone;

            // Represents the path to the word template.
            object pathWordTemplate = Settings.Default.WordTemplateFilePath;

            // These objects represent word settings.
            object readOnly;
            object isVisible;

            // Set Word to be (not) visble.
            if (visible)
            {
                readOnly = false;
                isVisible = true;

                // Set Word to be visble.
                this.wordApp.Visible = true;
            }
            else
            {
                readOnly = false;
                isVisible = false;

                // Set Word to be not visble.
                this.wordApp.Visible = false;
            }

            // Open the word document.
            this.wordDoc = this.wordApp.Documents.Open(ref pathWordTemplate, ref this.oMissing, ref readOnly, ref this.oMissing, ref this.oMissing,
                                                       ref this.oMissing, ref this.oMissing, ref this.oMissing, ref this.oMissing, ref this.oMissing, ref this.oMissing, ref isVisible, ref this.oMissing, ref this.oMissing, ref this.oMissing, ref this.oMissing);

            // Activate the document.
            this.wordDoc.Activate();

            // check if saveable, before filling in datas
            this.wordDoc.SaveAs(Path.Combine(Settings.Default.BillFolderPath, Resources.BillFileName));

            // Find Place Holders and Replace them with values.
            this.FindAndReplace(this.wordApp, "<Anrede>", billItemEditViewModel.CurrentBillDetailViewModel.Title.ToString());

            if (string.IsNullOrEmpty(billItemEditViewModel.CurrentBillDetailViewModel.CompanyName))
            {
                var range = this.wordDoc.Content;

                if (range.Find.Execute("<Firmenname>"))
                {
                    range.Expand(WdUnits.wdSentence); // or change to .wdSentence or .wdParagraph
                    range.Delete();
                }
            }
            else
            {
                this.FindAndReplace(this.wordApp, "<Firmenname>", billItemEditViewModel.CurrentBillDetailViewModel.CompanyName);
            }

            this.FindAndReplace(this.wordApp, "<Vorname>", billItemEditViewModel.CurrentBillDetailViewModel.FirstName);
            this.FindAndReplace(this.wordApp, "<Nachname>", billItemEditViewModel.CurrentBillDetailViewModel.LastName);
            this.FindAndReplace(this.wordApp, "<Strasse>", billItemEditViewModel.CurrentBillDetailViewModel.Street);
            this.FindAndReplace(this.wordApp, "<Hausnummer>", billItemEditViewModel.CurrentBillDetailViewModel.HouseNumber);
            this.FindAndReplace(this.wordApp, "<PLZ>", billItemEditViewModel.CurrentBillDetailViewModel.PostalCode);
            this.FindAndReplace(this.wordApp, "<Ort>", billItemEditViewModel.CurrentBillDetailViewModel.City);
            this.FindAndReplace(this.wordApp, "<Rechnungsart>", billItemEditViewModel.CurrentBillDetailViewModel.KindOfBill.ToString());
            this.FindAndReplace(this.wordApp, "<Datum>", billItemEditViewModel.CurrentBillDetailViewModel.Date);
            this.FindAndReplace(this.wordApp, "<Rechnungsnummer>", billItemEditViewModel.CurrentBillDetailViewModel.Id);
            this.FindAndReplace(this.wordApp, "<Kundennummer>", billItemEditViewModel.CurrentBillDetailViewModel.ClientId);
            this.FindAndReplace(this.wordApp, "<Netto>", string.Format("{0:N2} €", billItemEditViewModel.NettoSum));
            this.FindAndReplace(this.wordApp, "<MwSt%> ", billItemEditViewModel.CurrentBillDetailViewModel.VatPercentage);
            this.FindAndReplace(this.wordApp, "<MwSt>", string.Format("{0:N2} €", billItemEditViewModel.VatSum));
            this.FindAndReplace(this.wordApp, "<Brutto>", string.Format("{0:N2} €", billItemEditViewModel.BruttoSum));
            this.FindAndReplace(this.wordApp, "<Angebot>", Settings.Default.Offer);

            // The bookmark where the table will be inserted.
            object bookmarkTableStart = "tableStart";

            // Creates a new instance of a table.
            Range wrdRng = this.wordDoc.Bookmarks.get_Item(ref bookmarkTableStart).Range;

            // Adds the table to the word document.
            Table objTable = this.wordDoc.Tables.Add(wrdRng, billItemEditViewModel.BillItemDetailViewModels.Count + 1, 7, ref this.oMissing, ref this.oMissing);

            // Sets the preferred width for the columns
            objTable.Columns.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
            // Pos
            objTable.Columns[1].PreferredWidth = 6;
            // Article number
            objTable.Columns[2].PreferredWidth = 10;
            // Description
            objTable.Columns[3].PreferredWidth = 45;
            // Amount
            objTable.Columns[4].PreferredWidth = 6;
            // Price
            objTable.Columns[5].PreferredWidth = 11;
            // Discount
            objTable.Columns[6].PreferredWidth = 8;
            // Sum
            objTable.Columns[7].PreferredWidth = 14;

            // Sets the preferred width of the whole table and centers the table
            objTable.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
            objTable.PreferredWidth = 100;
            objTable.Rows.Alignment = WdRowAlignment.wdAlignRowCenter;

            // Sets up the header for the cell
            objTable.Rows[1].Range.Font.Bold = 1;
            objTable.Rows[1].Range.Font.Size = 11;
            objTable.Rows[1].Range.Font.Name = "Calibri";
            objTable.Rows[1].AllowBreakAcrossPages = 0;
            objTable.Cell(1, 1).Range.Text = "Pos.";
            objTable.Cell(1, 2).Range.Text = "Artikelnr.";
            objTable.Cell(1, 3).Range.Text = "Bezeichnung";
            objTable.Cell(1, 4).Range.Text = "Anz.";
            objTable.Cell(1, 5).Range.Text = "Preis";
            objTable.Cell(1, 6).Range.Text = "Rabatt";
            objTable.Cell(1, 7).Range.Text = "Gesamtpreis";

            // Changes the background color of the header and centers the text
            for (int i = 1; i <= 7; i++)
            {
                objTable.Cell(1, i).Range.Shading.BackgroundPatternColor = (WdColor)ColorTranslator.ToOle(Color.LightGray);
                objTable.Cell(1, i).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            }

            // Fills in all the billDetails and sets the row height.
            for (int i = 2; i <= billItemEditViewModel.BillItemDetailViewModels.Count + 1; i++)
            {
                objTable.Cell(i, 1).Range.Text = billItemEditViewModel.BillItemDetailViewModels[i - 2].Position.ToString();
                objTable.Cell(i, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                objTable.Cell(i, 2).Range.Text = billItemEditViewModel.BillItemDetailViewModels[i - 2].ArticleNumber.ToString();
                objTable.Cell(i, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                objTable.Cell(i, 3).Range.Text = billItemEditViewModel.BillItemDetailViewModels[i - 2].Description;
                objTable.Cell(i, 4).Range.Text = string.Format("{0:####.##}", billItemEditViewModel.BillItemDetailViewModels[i - 2].Amount);
                objTable.Cell(i, 4).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                objTable.Cell(i, 5).Range.Text = string.Format("{0:N2} €", billItemEditViewModel.BillItemDetailViewModels[i - 2].Price);
                objTable.Cell(i, 5).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
                objTable.Cell(i, 6).Range.Text = string.Format("{0:#0} %", billItemEditViewModel.BillItemDetailViewModels[i - 2].Discount);
                objTable.Cell(i, 6).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                objTable.Cell(i, 7).Range.Text = string.Format("{0:N2} €", billItemEditViewModel.BillItemDetailViewModels[i - 2].Sum);
                objTable.Cell(i, 7).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
                objTable.Rows[i].AllowBreakAcrossPages = 0;
            }

            // Sets the border
            objTable.Rows[1].Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
            objTable.Rows[1].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
            objTable.Rows[billItemEditViewModel.BillItemDetailViewModels.Count + 1].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;

            for (int i = 1; i <= 7; i++)
            {
                objTable.Columns[i].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                objTable.Columns[i].Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
            }

            // save again when all datas were filled in
            this.wordDoc.SaveAs(Path.Combine(Settings.Default.BillFolderPath, Resources.BillFileName));
        }

        public bool PrintDocument()
        {
            bool printed = false;

            using (PrintDialog printDialog = new PrintDialog())
            {
                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    this.wordApp.ActivePrinter = printDialog.PrinterSettings.PrinterName;
                    this.wordApp.ActiveDocument.PrintOut();
                    printed = true;
                }
            }

            return printed;
        }

        public void CloseDocument()
        {
            try
            {
                this.wordDoc.Close(WdSaveOptions.wdDoNotSaveChanges);
                this.wordApp.Quit();
            }
            catch
            {
                // do nothing
            }
        }

        #endregion



        /// <summary>
        ///     Finds the textmarks and replaces them with the desired value.
        /// </summary>
        /// <param name="WordApp">
        ///     The word application.
        /// </param>
        /// <param name="findText">
        ///     the textmark.
        /// </param>
        /// <param name="replaceWithText">
        ///     The text that should be inserted.
        /// </param>
        private void FindAndReplace(_Application WordApp, object findText, object replaceWithText)
        {
            // options
            object matchCase = true;
            object matchWholeWord = true;
            object matchWildCards = false;
            object matchSoundsLike = false;
            object matchAllWordForms = false;
            object forward = true;
            object format = false;
            object matchKashida = false;
            object matchDiacritics = false;
            object matchAlefHamza = false;
            object matchControl = false;
            object replace = 2;
            object wrap = 1;

            // execute find and replace
            WordApp.Selection.Find.Execute(ref findText, ref matchCase, ref matchWholeWord, ref matchWildCards, ref matchSoundsLike,
                                           ref matchAllWordForms, ref forward, ref wrap, ref format, ref replaceWithText, ref replace,
                                           ref matchKashida, ref matchDiacritics, ref matchAlefHamza, ref matchControl);
        }
    }
}