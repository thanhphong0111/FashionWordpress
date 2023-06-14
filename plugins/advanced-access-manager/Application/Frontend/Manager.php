<?php

/**
 * ======================================================================
 * LICENSE: This file is subject to the terms and conditions defined in *
 * file 'license.txt', which is part of this source code package.       *
 * ======================================================================
 */

/**
 * AAM frontend manager
 * 
 * @package AAM
 * @author Vasyl Martyniuk <vasyl@vasyltech.com>
 */
class AAM_Frontend_Manager {

    /**
     * Instance of itself
     * 
     * @var AAM_Frontend_Manager
     * 
     * @access private 
     */
    private static $_instance = null;
    
    /**
     * pre_get_posts flag
     */
    protected $skip = false;

    /**
     * Construct the manager
     * 
     * @return void
     * 
     * @access public
     */
    public function __construct() {
        if (AAM_Core_Config::get('frontend-access-control', true)) {
            //login hook
            add_action('wp_login', array($this, 'login'), 10, 2);
        
            //control WordPress frontend
            add_action('wp', array($this, 'wp'), 999);
            add_action('404_template', array($this, 'themeRedirect'), 999);
            //filter navigation pages & taxonomies
            add_filter('get_pages', array($this, 'thePosts'));
            add_filter('wp_get_nav_menu_items', array($this, 'getNavigationMenu'));
            //widget filters
            add_filter('sidebars_widgets', array($this, 'widgetFilter'), 999);
            //get control over commenting stuff
            add_filter('comments_open', array($this, 'commentOpen'), 10, 2);
            //user login control
            add_filter('wp_authenticate_user', array($this, 'authenticate'), 1, 2);
            //add post filter for LIST restriction
            add_filter('the_posts', array($this, 'thePosts'), 999, 2);
            if (AAM_Core_Config::get('large-post-number', false)) {
                add_action('pre_get_posts', array($this, 'preparePostQuery'));
            }
            
            //password protected filter
            add_filter('post_password_required', array($this, 'isProtected'), 10, 2);
            
            //filter post content
            add_filter('the_content', array($this, 'theContent'), 999);
            
            //manage AAM shortcode
            add_shortcode('aam', array($this, 'processShortcode'));
            
            //admin bar
            $this->checkAdminBar();
        }
        
        if (AAM_Core_Request::get('action') == 'aam-auth') {
            $this->doubleAuthentication();
        }
        
        //security controls
        add_action('login_form_login', array($this, 'loginSubmit'), 1);
    }
    
    /**
     * 
     */
    public function loginSubmit() {
        //Login Timeout
        if (AAM_Core_Config::get('login-timeout', false)) {
            @sleep(intval(AAM_Core_Config::get('security.login.timeout', 1)));
        }
        
        //Brute Force Lockout
        if (AAM_Core_Config::get('brute-force-lockout', false)) {
            $this->updateLoginCounter(1);
        }
    }
    
    /**
     * 
     * @param type $username
     * @param type $user
     */
    public function login($username, $user = null) { /* CodePinch UE Fix */
        if (is_a($user, 'WP_User')) {
            $this->updateLoginCounter(-1);
            
            AAM_Core_API::deleteOption('aam-user-switch-' . $user->ID);
            
            $subject = new AAM_Core_Subject_User($user->ID);
            $object  = $subject->getObject('loginRedirect');
            
            //if Login redirect is defined
            $type     = $object->get('login.redirect.type');
            $redirect = AAM_Core_Request::request('aam_redirect');
            
            if (!empty($redirect)) {
                AAM_Core_API::redirect($redirect);
            }elseif (!empty($type) && $type !== 'default') {
                $redirect = $object->get("login.redirect.{$type}");
                AAM_Core_API::redirect($redirect);
            }
        }
    }
    
    /**
     * 
     * @param type $increment
     */
    protected function updateLoginCounter($increment) {
        $attempts = get_transient('aam-login-attemtps');
        
        if ($attempts !== false) {
            $timeout = get_option('_transient_timeout_aam-login-attemtps') - time();
            $attempts = intval($attempts) + $increment;
        } else {
            $attempts = 1;
            $timeout = strtotime(
                '+' . AAM_Core_Config::get('security.login.period', '2 minutes')
            ) - time();
        }
        
        if ($attempts >= AAM_Core_Config::get('security.login.attempts', 20)) {
            wp_safe_redirect(site_url('index.php'));
            exit;
        } else {
            set_transient('aam-login-attemtps', $attempts, $timeout);
        }
    }
    
    /**
     * Control User Block flag
     *
     * @param WP_Error $user
     *
     * @return WP_Error|WP_User
     *
     * @access public
     */
    public function authenticate($user) {
        if ($user->user_status == 1) {
            $user = new WP_Error();
            
            $message  = '[ERROR]: User is locked. Please contact your website ';
            $message .= 'administrator.';
            
            $user->add(
                'authentication_failed', 
                AAM_Backend_View_Helper::preparePhrase($message, 'strong')
            );
        } elseif (AAM_Core_Config::get('login-ip-track', false)) {
            $baseIp = get_user_meta($user->ID, 'aam-login-ip', true);
            $ip     = AAM_Core_Request::server('REMOTE_ADDR');
            
            if (empty($baseIp)) {
                update_user_meta($user->ID, 'aam-login-ip', $ip);
            }
            
            if (!empty($baseIp) && ($baseIp != $ip)) {
                $key = get_password_reset_key($user);
                update_user_meta($user->ID, 'aam-login-key', $key);
                
                if ( is_multisite() ) {
                    $blogname = get_network()->site_name;
                } else {
                    $blogname = wp_specialchars_decode(get_option('blogname'), ENT_QUOTES);
                }

        	$title = sprintf( __('[%s] Double Authentication'), $blogname );
                
                $message  = sprintf(__('Someone was trying to login from the different IP address %s:'), $ip) . "\r\n\r\n";
                $message .= sprintf(__('Website: %s'), network_home_url( '/' )) . "\r\n";
                $message .= sprintf(__('Username: %s'), $user->user_login) . "\r\n\r\n";
                $message .= __('Visit the following address to authorize the login:') . "\r\n\r\n";
                $message .= '<' . network_site_url("index.php?action=aam-auth&key={$key}&login={$user->user_login}") . ">\r\n";
                
                wp_mail($user->user_email, $title, $message);
                
                $user = new WP_Error();
            
                $message  = '[ERROR]: Double authentication is required. ';
                $message .= 'Please check your email or enter username and ';
                $message .= 'password again to resend the email.';

                $user->add(
                    'authentication_failed', 
                    AAM_Backend_View_Helper::preparePhrase($message, 'strong')
                );
            }
        }

        return $user;
    }
    
    /**
     * 
     */
    protected function doubleAuthentication() {
        $login = AAM_Core_Request::get('login');
        $key   = AAM_Core_Request::get('key');
        $user  = get_user_by('login', $login);
        
        if (is_a($user, 'WP_User')) {
            $stored = get_user_meta($user->ID, 'aam-login-key', true);
            
            if ($stored == $key) {
                update_user_meta($user->ID, 'aam-login-ip', AAM_Core_Request::server('REMOTE_ADDR'));
                delete_user_meta($user->ID, 'aam-login-key');
                wp_safe_redirect(site_url('wp-login.php'));
                exit;
            }
        }
        
        wp_safe_redirect(site_url('index.php'));
        exit;
    }
    
    /**
     * 
     * @param type $response
     * @param WP_Post $post
     * @return type
     */
    public function isProtected($response, $post) {
        if (is_a($post, 'WP_Post')) {
            $object = AAM::getUser()->getObject('post', $post->ID);

            if ($object->has('frontend.protected')) {
                $hasher = new PasswordHash( 8, true );
                $hash   = wp_unslash(AAM_Core_Request::cookie('wp-postpass_' . COOKIEHASH));

                if (empty($hash)) {
                    $response = true;
                } else {
                    $response = !$hasher->CheckPassword(
                            $object->get('frontend.password'), $hash 
                    );
                }
            }
        }
        
        return $response;
    }

    /**
     * Main frontend access control hook
     *
     * @return void
     *
     * @access public
     * @global WP_Post $post
     */
    public function wp() {
        $post = $this->getCurrentPost();
        if (is_a($post, 'WP_Post')) {
            $this->checkPostReadAccess($post);
        }
    }
    
    /**
     * Theme redirect
     * 
     * Super important function that cover the 404 redirect that triggered by theme
     * when page is not found. This covers the scenario when page is restricted from
     * listing and read.
     * 
     * @global type $wp_query
     * 
     * @param type $template
     * 
     * @return string
     * 
     * @access public
     */
    public function themeRedirect($template) {
        global $wp_query;
        
        $object = (isset($wp_query->queried_object) ? $wp_query->queried_object : 0);
        if ($object && is_a($object, 'WP_Post')) {
            $this->checkPostReadAccess($object);
        }
        
        return $template;
    }
    
    /**
     * 
     * @global type $wp_query
     * @return type
     */
    protected function getCurrentPost() {
        global $wp_query, $post;
        
        $current = null;
        
        if (!empty($wp_query->queried_object)) {
            $current = $wp_query->queried_object;
        } elseif (!empty($wp_query->post)) {
            $current = $wp_query->post;
        } elseif (!empty($wp_query->query['name']) && !empty($wp_query->posts)) {
            //Important! Cover the scenario of NOT LIST but ALLOW READ
            foreach($wp_query->posts as $post) {
                if ($post->post_name == $wp_query->query['name']) {
                    $current = $post;
                    break;
                }
            }
        }
        
        return (is_a($current, 'WP_Post') ? $current : null);
    }
    
    /**
     * Check post read access
     * 
     * @param WP_Post $post
     * 
     * @return void
     * 
     * @access protected
     */
    protected function checkPostReadAccess($post) {
        $object = AAM::getUser()->getObject('post', $post->ID);
        $read   = $object->has('frontend.read');
        $others = $object->has('frontend.read_others');
        
        $restrict = apply_filters(
                'aam-check-post-read-access-filer',
                ($read || ($others && !$this->isAuthor($post))),
                $object
        );

        if ($restrict) {
            AAM_Core_API::reject(
                'frontend', 
                array(
                    'hook'   => 'post_read', 
                    'action' => 'frontend.read', 
                    'post'   => $post
                )
            );
        }
        
        //check post redirect
        if ($object->has('frontend.redirect')) {
            AAM_Core_API::redirect($object->get('frontend.location'));
        }
        
        //trigger any action 
        do_action('aam-wp-action', $object);
    }
    
    /**
     * Filter posts from the list
     *  
     * @param array $posts
     * 
     * @return array
     * 
     * @access public
     */
    public function thePosts($posts) {
        $current = $this->getCurrentPost();
        
        if (is_array($posts)) {
            foreach ($posts as $i => $post) {
                if ($current && ($current->ID == $post->ID)) { continue; } 
                
                $object = AAM::getUser()->getObject('post', $post->ID);
                $list   = $object->has('frontend.list');
                $others = $object->has('frontend.list_others');
                
                if ($list || ($others && !$this->isAuthor($post))) {
                    unset($posts[$i]);
                }
            }
        }

        return $posts;
    }

    /**
     * Filter Navigation menu
     *
     * @param array $pages
     *
     * @return array
     *
     * @access public
     */
    public function getNavigationMenu($pages) {
        if (is_array($pages)) {
            $user = AAM::getUser();
            foreach ($pages as $i => $page) {
                if ($page->type == 'post_type') {
                    $object = $user->getObject('post', $page->object_id);
                    $list   = $object->has('frontend.list');
                    $others = $object->has('frontend.list_others');
                    
                    if ($list || ($others && !$this->isAuthor($page))) {
                        unset($pages[$i]);
                    }
                }
            }
        }

        return $pages;
    }

    /**
     * Filter Frontend widgets
     *
     * @param array $widgets
     *
     * @return array
     *
     * @access public
     */
    public function widgetFilter($widgets) {
        return AAM::getUser()->getObject('metabox')->filterFrontend($widgets);
    }

    /**
     * Control Frontend commenting freature
     *
     * @param boolean $open
     * @param int $post_id
     *
     * @return boolean
     *
     * @access public
     */
    public function commentOpen($open, $post_id) {
        $object = AAM::getUser()->getObject('post', $post_id);
        
        if ($object->has('frontend.comment')) {
            $open = false;
        }

        return $open;
    }
    
    /**
     * Check admin bar
     * 
     * Make sure that current user can see admin bar
     * 
     * @return void
     * 
     * @access public
     */
    public function checkAdminBar() {
        $caps = AAM_Core_API::getAllCapabilities();
        
        if (isset($caps['show_admin_bar'])) {
            if (!AAM::getUser()->hasCapability('show_admin_bar')) {
                show_admin_bar(false);
            }
        }
    }

    /**
     * 
     * @param type $query
     */
    public function preparePostQuery($query) {
        if ($this->skip === false) {
            $filtered = array();
            $key      = $this->getQueryType($query) . '__not_in_frontend';
            
            if (AAM_Core_Cache::has($key)) {
                $filtered = AAM_Core_Cache::get($key);
            } else {
                foreach ($this->fetchPosts($query) as $id) {
                    $object = AAM::getUser()->getObject('post', $id);
                    $list   = $object->has('frontend.list');
                    $others = $object->has('frontend.list_others');

                    if ($list || ($others && !$this->isAuthor($object->getPost()))) {
                        $filtered[] = $id;
                    }
                }
                AAM_Core_Cache::set(AAM::getUser(), $key, $filtered);
            }
            
            if (isset($query->query_vars['post__not_in'])) {
                $query->query_vars['post__not_in'] = array_merge(
                        $query->query_vars['post__not_in'], $filtered
                );
            } else {
                $query->query_vars['post__not_in'] = $filtered;
            }
        }
    }

    /**
     * 
     * @param type $query
     * @return type
     */
    protected function fetchPosts($query) {
        $this->skip = true;
        
        $posts = get_posts(array(
            'post_type'   => $this->getQueryType($query),
            'numberposts' => -1,
            'fields'      => 'ids'
        ));
                    
        $this->skip = false;
        
        return $posts;
    }
    
    /**
     * 
     * @param type $query
     * @return type
     */
    protected function getQueryType($query) {
        if (!empty($query->query['post_type'])) {
            $postType = $query->query['post_type'];
        } elseif (!empty($query->query_vars['post_type'])) {
            $postType = $query->query_vars['post_type'];
        } else {
            $postType = 'post';
        }
        
        return (is_string($postType) ? $postType : 'post');
    }
    
    /**
     * 
     * @global WP_Post $post
     * @param type $content
     * 
     * @return string
     * 
     * @access public
     */
    public function theContent($content) {
        global $post;
        
        if (is_a($post, 'WP_Post')) {
            $object = AAM::getUser()->getObject('post', $post->ID);
            if ($object->has('frontend.limit')) {
                $teaser  = AAM::getUser()->getObject('teaser');
                $message = $teaser->get('frontend.teaser.message');
                $excerpt = $teaser->get('frontend.teaser.excerpt');
                
                $html  = (intval($excerpt) ? $post->post_excerpt : '');
                $html .= stripslashes($message);
                $content = do_shortcode($html);
            }
        }
        
        return $content;
    }
    
    /**
     * 
     * @param type $args
     * @param type $content
     * @return type
     */
    public function processShortcode($args, $content) {
        $shortcode = new AAM_Shortcode_Factory($args, $content);
        
        return $shortcode->process();
    }
    
    /**
     * Check if user is post author
     * 
     * @param WP_Post $post
     * 
     * @return boolean
     * 
     * @access protected
     */
    protected function isAuthor($post) {
        return ($post->post_author == get_current_user_id());
    }

    /**
     * Bootstrap the manager
     * 
     * @return void
     * 
     * @access public
     */
    public static function bootstrap() {
        if (is_null(self::$_instance)) {
            self::$_instance = new self;
        }
    }

}