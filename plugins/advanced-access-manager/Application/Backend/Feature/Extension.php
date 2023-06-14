<?php

/**
 * ======================================================================
 * LICENSE: This file is subject to the terms and conditions defined in *
 * file 'license.txt', which is part of this source code package.       *
 * ======================================================================
 */

/**
 * Backend extension manager
 * 
 * @package AAM
 * @author Vasyl Martyniuk <vasyl@vasyltech.com>
 */
class AAM_Backend_Feature_Extension extends AAM_Backend_Feature_Abstract {
    
    /**
     * @inheritdoc
     */
    public static function getAccessOption() {
        return 'feature.extension.capability';
    }
    
    /**
     * @inheritdoc
     */
    public static function getTemplate() {
        return 'extension.phtml';
    }
    
    /**
     * Install an extension
     * 
     * @param string $storedLicense
     * 
     * @return string
     * 
     * @access public
     */
    public function install($storedLicense = null) {
        $repo    = AAM_Extension_Repository::getInstance();
        $license = AAM_Core_Request::post('license', $storedLicense);
        
        //download the extension from the server first
        $package = AAM_Extension_Server::download($license);
        
        if (is_wp_error($package)) {
            $response = array(
                'status' => 'failure', 'error'  => $package->get_error_message()
            );
        }elseif ($error = $repo->checkDirectory()) {
            $response = $this->installFailureResponse($error, $package);
            $repo->storeLicense($package, $license);
        } elseif (empty($package->content)) { //any unpredictable scenario
            $response = array(
                'status' => 'failure', 
                'error'  => 'Download failure. Please try again or contact us.'
            );
        } else { //otherwise install the extension
            $result = $repo->add(base64_decode($package->content));
            if (is_wp_error($result)) {
                $response = $this->installFailureResponse(
                        $result->get_error_message(), $package
                );
            } else {
                $response = array('status' => 'success');
            }
            $repo->storeLicense($package, $license);
        }
        
        return json_encode($response);
    }
    
    /**
     * Update the extension
     * 
     * @return string
     * 
     * @access public
     */
    public function update() {
        $extension = AAM_Core_Request::post('extension');
        $list      = AAM_Core_Compatibility::getExtensionList();
        
        if (!empty($list[$extension]['license'])) {
            $response = $this->install($list[$extension]['license']);
        } else {
            $response = json_encode(array(
                'status' => 'failure', 
                'error'  => __('License key is missing.', AAM_KEY)
            ));
        }
        
        return $response;
    }
    
    /**
     * 
     * @return type
     */
    public function check() {
        AAM_Extension_Repository::getInstance()->check();
        
        return json_encode(array('status' => 'success'));
    }
    
    /**
     * 
     * @param type $type
     * @return type
     */
    public function getList($type) {
        $response = array();
        
        foreach(AAM_Extension_Repository::getInstance()->getList() as $item) {
            if ($item['type'] == $type) {
                $response[] = $item;
            }
        }
        
        return $response;
    }
    
    /**
     * Install extension failure response
     * 
     * In case the file system fails, AAM allows to download the extension for
     * manual installation
     * 
     * @param string   $error
     * @param stdClass $package
     * 
     * @return array
     * 
     * @access protected
     */
    protected function installFailureResponse($error, $package) {
        return array(
            'status'  => 'failure',
            'error'   => $error,
            'title'   => $package->title,
            'content' => $package->content
        );
    }
    
    /**
     * Register Extension feature
     * 
     * @return void
     * 
     * @access public
     */
    public static function register() {
        if (is_main_site()) {
            $cap = AAM_Core_Config::get(self::getAccessOption(), 'administrator');

            AAM_Backend_Feature::registerFeature((object) array(
                'uid'          => 'extension',
                'position'     => 999,
                'title'        => __('Extensions', AAM_KEY),
                'capability'   => $cap,
                'notification' => self::getNotification(),
                'subjects'     => array(
                    'AAM_Core_Subject_Role',
                    'AAM_Core_Subject_User',
                    'AAM_Core_Subject_Visitor',
                ),
                'view'         => __CLASS__
            ));
        }
    }
    
    /**
     * 
     * @return int
     */
    protected static function getNotification() {
        $count = 0;
        
        foreach(AAM_Extension_Repository::getInstance()->getList() as $extension) {
            if ($extension['status'] == AAM_Extension_Repository::STATUS_UPDATE) {
                $count++;
            }
        }
        
        return ($count ? $count : 'NEW');
    }

}