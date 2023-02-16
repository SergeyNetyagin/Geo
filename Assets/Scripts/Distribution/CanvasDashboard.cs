using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using VertexStudio.Generic;

namespace VostokVR.Geo {

    public class CanvasDashboard : MonoBehaviour {

        public static CanvasDashboard Instance { get; private set; }

        [Space( 10 ), SerializeField]
        private InputField input_field_day;

        [SerializeField]
        private InputField input_field_month;

        [SerializeField]
        private InputField input_field_year;

        [Space( 10 ), SerializeField]
        private InputField input_field_source_folder;

        [SerializeField]
        private InputField input_field_destination_folder;

        [SerializeField]
        private InputField input_field_asset_bundles_folder;

        [SerializeField]
        private Toggle checkbox_include_asset_bundles;

        [SerializeField]
        private Text text_processing_time;

        [Space( 10 ), SerializeField]
        private RectTransform panel_status;

        [SerializeField]
        private Text text_status;

        [Space( 10 ), SerializeField]
        private RectTransform panel_progress;

        [SerializeField]
        private Text text_progress_title;

        [SerializeField]
        private Text text_progress_description;

        [Space( 10 ), SerializeField]
        private RectTransform panel_message;

        [SerializeField]
        private Text text_message_title;

        [SerializeField]
        private Text text_message_description;

        [Space( 10 ), SerializeField]
        private float check_storage_multiplier = 2f;

        public string GetSourceFolder() { return input_field_source_folder.text; }
        public string GetDestinationFolder() { return input_field_destination_folder.text; }
        public string GetAssetBundlesFolder() { return input_field_asset_bundles_folder.text; }

        public bool Include_asset_bundles { get { return checkbox_include_asset_bundles.isOn; } }

        // #################################################################################################################################################################################################################################################
        private void Awake() {
        
            Instance = this;
        }

        // #################################################################################################################################################################################################################################################
        private void Start() {
        
            input_field_day.text = "1";
            input_field_day.textComponent.text = "1";

            input_field_month.text = "1";
            input_field_month.textComponent.text = "1";

            input_field_year.text = DateTime.Now.Year.ToString();
            input_field_year.textComponent.text = DateTime.Now.Year.ToString();

            panel_message.gameObject.SetActive( false );
            panel_progress.gameObject.SetActive( false );

            CheckForAssetBundlesFolder();

            text_status.text = string.Empty;
            text_processing_time.text = string.Empty;
        }

        // #################################################################################################################################################################################################################################################
        public void InitializeSourceFolderField() {

            input_field_source_folder.text = DistributionControl.Instance.IsFirstUsing() ? string.Empty : DistributionControl.Instance.GetSourcePath();
            input_field_source_folder.textComponent.text = input_field_source_folder.text;

            UpdateStatusInfo();
        }

        // #################################################################################################################################################################################################################################################
        public void InitializeDestinationFolderField() {

            input_field_destination_folder.text = DistributionControl.Instance.IsFirstUsing() ? string.Empty : DistributionControl.Instance.GetDestinationPath();
            input_field_destination_folder.textComponent.text = input_field_destination_folder.text;

            UpdateStatusInfo();
        }

        // #################################################################################################################################################################################################################################################
        public void CheckForSourceFolder( InputField input_field ) {

            if( string.IsNullOrEmpty( input_field.text ) ) { 
            
                return;
            }

            bool directory_exists = false;

            try { 

                if( Directory.Exists( input_field.text ) ) { 
                
                    directory_exists = true;
                }
            }

            catch { 
            
                directory_exists = false;
            }

            if( !directory_exists ) { 
            
                ShowMessage(
                    
                    "SOURCE FOLDER ERROR", 
                    "Cannot find the specified source folder!" + Environment.NewLine + 
                    "Check the folder existance or correct the path."
                );

                return;
            }

            if( !string.IsNullOrEmpty( DistributionControl.Instance.GetDrive( input_field.text ) ) ) { 

                DistributionControl.Instance.SaveSourceFolder( input_field.text );
            }

            else { 
                
                ShowMessage(
                    
                    "SOURCE DRIVE ERROR", 
                    "Cannot find the drive letter in the path!" + Environment.NewLine + 
                    "Check the drive existance or correct the path."
                );
            }
        }

        // #################################################################################################################################################################################################################################################
        public void CheckForDestinationFolder( InputField input_field ) {

            if( string.IsNullOrEmpty( input_field.text ) ) { 
            
                return;
            }

            bool directory_exists = false;

            try { 

                if( Directory.Exists( input_field.text ) ) { 
                
                    directory_exists = true;
                }
            }

            catch { 
            
                directory_exists = false;
            }

            if( directory_exists ) { 
            
                ShowMessage( 
                    
                    "DESTINATION FOLDER PROBLEM", 
                    "The specified destination folder already exist." + Environment.NewLine + 
                    "Perhaps it contains an important data." + Environment.NewLine + 
                    "Rename the existing folder or remove it before zipping action."
                );

                return;
            }

            if( !string.IsNullOrEmpty( DistributionControl.Instance.GetDrive( input_field.text ) ) ) { 

                DistributionControl.Instance.SaveDestinationFolder( input_field.text );
            }

            else { 
                
                ShowMessage(
                    
                    "DESTINATION DRIVE ERROR", 
                    "Cannot find the drive letter in the path!" + Environment.NewLine + 
                    "Check the drive existance or correct the path."
                );
            }
        }

        // #################################################################################################################################################################################################################################################
        public void CheckForAssetBundlesFolder() {

            input_field_asset_bundles_folder.text = ConfigControl.Instance.GetAssetBundlesFolder();
            input_field_asset_bundles_folder.textComponent.text = input_field_asset_bundles_folder.text;
        }

        // #################################################################################################################################################################################################################################################
        public void UpdateStatusInfo() {

            int available_space = DistributionControl.Instance.GetDriveAvailableSpaceInGygabytes( input_field_destination_folder.text );

            int required_space = (
                
                checkbox_include_asset_bundles.isOn ? 
                (int) (DistributionControl.Instance.GetRequiredSpaceInGygabytes( input_field_source_folder.text, input_field_asset_bundles_folder.text ) * check_storage_multiplier) : 
                (int) (DistributionControl.Instance.GetRequiredSpaceInGygabytes( input_field_source_folder.text, null ) * check_storage_multiplier)
            );

            if( required_space > 0f ) { 
            
                text_status.text = "Drive " + DistributionControl.Instance.GetDrive( input_field_destination_folder.text ) + " available space: " + available_space + " Gb" + "; required space: " + required_space + " Gb";
            }

            else { 

                text_status.text = "Drive " + DistributionControl.Instance.GetDrive( input_field_destination_folder.text ) + " available space: " + available_space + " Gb";
            }

            if( checkbox_include_asset_bundles.isOn ) { 
                
                text_processing_time.text = "Processig time is about " + (DistributionControl.Instance.App_files_processing_time_in_mins + DistributionControl.Instance.Asset_bundles_processing_time_in_mins) + " mins";
            }

            else { 
                
                text_processing_time.text = "Processig time is about " + DistributionControl.Instance.App_files_processing_time_in_mins + " mins";
            }
        }

        // #################################################################################################################################################################################################################################################
        public void ShowMessage( string title, string description ) {

            text_message_title.text = title;
            text_message_description.text = description;

            panel_message.gameObject.SetActive( true );
        }

        // #################################################################################################################################################################################################################################################
        public void ShowProgress( string title, string description  ) {

            if( !panel_progress.gameObject.activeSelf ) { 
                
                panel_progress.gameObject.SetActive( true );
            }

            text_progress_title.text = title;
            text_progress_description.text = description;
        }

        // #################################################################################################################################################################################################################################################
        public void HideProgress() {

            if( panel_progress.gameObject.activeSelf ) { 
                
                panel_progress.gameObject.SetActive( false );
            }
        }

        // #################################################################################################################################################################################################################################################
        public int GetDay() {

            input_field_day.GetComponent<CheckIntValueControl>().CheckValue( input_field_day );

            int value = input_field_day.GetComponent<CheckIntValueControl>().Current_value;

            return value;
        }

        // #################################################################################################################################################################################################################################################
        public int GetMonth() {

            input_field_month.GetComponent<CheckIntValueControl>().CheckValue( input_field_month );

            int value = input_field_month.GetComponent<CheckIntValueControl>().Current_value;

            return value;
        }

        // #################################################################################################################################################################################################################################################
        public int GetYear() {

            input_field_year.GetComponent<CheckIntValueControl>().CheckValue( input_field_year );

            int value = input_field_year.GetComponent<CheckIntValueControl>().Current_value;

            return value;
        }
    }
}