<?php

/**
 * ======================================================================
 * LICENSE: This file is subject to the terms and conditions defined in *
 * file 'license.txt', which is part of this source code package.       *
 * ======================================================================
 */

/**
 * AAM core API
 * 
 * @package AAM
 * @author Vasyl Martyniuk <vasyl@vasyltech.com>
 */
final class AAM_Core_API {

    /**
     * Get option
     *
     * @param string $option
     * @param mixed  $default
     * @param int    $blog_id
     *
     * @return mixed
     *
     * @access public
     * @static
     */
    public static function getOption($option, $default = FALSE, $blog_id = null) {
        if (is_multisite()) {
            if (is_null($blog_id)) {
                $blog = get_current_blog_id();
            } elseif ($blog_id == 'site') {
                $blog = (defined('SITE_ID_CURRENT_SITE') ? SITE_ID_CURRENT_SITE : 1);
            } else {
                $blog = $blog_id;
            }
            $response = get_blog_option($blog, $option, $default);
        } else {
            $response = get_option($option, $default);
        }

        return $response;
    }

    /**
     * Update option
     *
     * @param string $option
     * @param mixed  $data
     * @param int    $blog_id
     *
     * @return bool
     *
     * @access public
     * @static
     */
    public static function updateOption($option, $data, $blog_id = null) {
        if (is_multisite()) {
            if (is_null($blog_id)) {
                $blog = get_current_blog_id();
            } elseif ($blog_id == 'site') {
                $blog = (defined('SITE_ID_CURRENT_SITE') ? SITE_ID_CURRENT_SITE : 1);
            } else {
                $blog = $blog_id;
            }
            $response = update_blog_option($blog, $option, $data);
        } else {
            $response = update_option($option, $data);
        }

        return $response;
    }

    /**
     * Delete option
     *
     * @param string $option
     * @param int    $blog_id
     * 
     * @return bool
     *
     * @access public
     * @static
     */
    public static function deleteOption($option, $blog_id = null) {
        if (is_multisite()) {
            if (is_null($blog_id)) {
                $blog = get_current_blog_id();
            } elseif ($blog_id == 'site') {
                $blog = (defined('SITE_ID_CURRENT_SITE') ? SITE_ID_CURRENT_SITE : 1);
            } else {
                $blog = $blog_id;
            }
            $response = delete_blog_option($blog, $option);
        } else {
            $response = delete_option($option);
        }

        return $response;
    }

    /**
     * Initiate HTTP request
     *
     * @param string $url Requested URL
     * @param bool $send_cookies Wheather send cookies or not
     * 
     * @return WP_Error|array
     * 
     * @access public
     */
    public static function cURL($url, $send_cookies = TRUE, $params = array()) {
        $header = array('User-Agent' => AAM_Core_Request::server('HTTP_USER_AGENT'));

        $cookies = AAM_Core_Request::cookie(null, array());
        $requestCookies = array();
        if (is_array($cookies) && $send_cookies) {
            foreach ($cookies as $key => $value) {
                //SKIP PHPSESSID - some servers don't like it for security reason
                if ($key !== session_name()) {
                    $requestCookies[] = new WP_Http_Cookie(array(
                        'name' => $key, 'value' => $value
                    ));
                }
            }
        }

        return wp_remote_request($url, array(
            'headers' => $header,
            'method'  => 'POST',
            'body'    => $params,
            'cookies' => $requestCookies,
            'timeout' => 10
        ));
    }
    
    /**
     * Get role list
     * 
     * @global WP_Roles $wp_roles
     * 
     * @return WP_Roles
     */
    public static function getRoles() {
        global $wp_roles;
        
        if (function_exists('wp_roles')) {
            $roles = wp_roles();
        } elseif(isset($wp_roles)) {
            $roles = $wp_roles;
        } else {
            $roles = $wp_roles = new WP_Roles();
        }
        
        return $roles;
    }
    
    /**
     * Return max capability level
     * 
     * @param array $caps
     * @param int   $default
     * 
     * @return int
     * 
     * @access public
     */
    public static function maxLevel($caps, $default = 0) {
        $levels = array($default);
        
        if (is_array($caps)) { //WP Error Fix bug report
            foreach($caps as $cap => $granted) {
                if ($granted && preg_match('/^level_([0-9]+)$/i', $cap, $match)) {
                    $levels[] = intval($match[1]);
                }
            }
        }
        
        return max($levels);
    }
    
    /**
     * Get all capabilities
     * 
     * Prepare and return list of all registered in the system capabilities
     * 
     * @return array
     * 
     * @access public
     */
    public static function getAllCapabilities() {
        $caps = array();
        
        foreach (self::getRoles()->role_objects as $role) {
            if (is_array($role->capabilities)) {
                $caps = array_merge($caps, $role->capabilities);
            }
        }
        
        return $caps;
    }
    
    /**
     * Reject the request
     *
     * Redirect or die the execution based on ConfigPress settings
     * 
     * @param string $area
     * @param array  $args
     *
     * @return void
     *
     * @access public
     */
    public static function reject($area = 'frontend', $args = array()) {
        $object = AAM::getUser()->getObject('redirect');
        $type   = $object->get("{$area}.redirect.type");
        
        if (!empty($type)) {
            $redirect = $object->get("{$area}.redirect.{$type}");
        } else { //ConfigPress setup
            $redirect = AAM_Core_Config::get(
                   "{$area}.access.deny.redirect", __('Access Denied', AAM_KEY)
            );
        }
        
        do_action('aam-rejected-action', $area, $args);
        
        self::redirect($redirect, $args);
    }
    
    /**
     * 
     * @param type $rule
     * @param type $args
     */
    public static function redirect($rule, $args = null) {
        if (filter_var($rule, FILTER_VALIDATE_URL)) {
            wp_redirect($rule);
        } elseif (preg_match('/^[\d]+$/', $rule)) {
            wp_safe_redirect(get_post_permalink($rule));
        } elseif (is_callable($rule)) {
            call_user_func($rule, $args);
        } elseif (!empty($args['callback']) && is_callable($args['callback'])) {
            call_user_func($args['callback'], $rule, '', array());
        } elseif (empty($args['skip-die'])) {
            wp_die($rule);
        }
        exit;
    }
    
    /**
     * Remove directory recursively
     * 
     * @param string $pathname
     * 
     * @return void
     * 
     * @access public
     */
    public static function removeDirectory($pathname) {
        $files = glob($pathname . '/*');
        
	foreach ($files as $file) {
		is_dir($file) ? self::removeDirectory($file) : @unlink($file);
	}
        
	@rmdir($pathname);
    }
    
    /**
     * 
     * @return type
     */
    public static function version() {
        if (file_exists(ABSPATH . 'wp-admin/includes/plugin.php')) {
            require_once ABSPATH . 'wp-admin/includes/plugin.php';
        }
        
        if (function_exists('get_plugin_data')) {
            $data    = get_plugin_data(dirname(__FILE__) . '/../../aam.php');
            $version = (isset($data['Version']) ? $data['Version'] : null);
        }
        
        return (!empty($version) ? $version : null);
    }
    
}