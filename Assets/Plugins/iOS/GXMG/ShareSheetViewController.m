// Displays the share sheet overlays onto the screen
// Params:
// - path: The path to the image that we want to share to other applications
// - arrowHeightPercentage: The location of the arrow on screen denoted in total screen height percentage
// - subjectLine: The subject line that will accompany the shareMessage in supported applications
// - shareMessage: The message that will accompany the shared photo in supported applications
// - Returns: void
void _displayIOSShareSheet(const char * path, float arrowHeightPercentage, const char * subjectLine, const char * shareMessage) {
    NSString *imagePath = [NSString stringWithUTF8String:path];
    
    UIImage *image = [UIImage imageWithContentsOfFile:imagePath];
    NSString *message = [NSString stringWithUTF8String:shareMessage];
    NSArray *postItems = @[message,image];
    
    [[UIApplication sharedApplication].keyWindow.rootViewController dismissViewControllerAnimated:NO completion:nil];
    
    UIActivityViewController *activityVc = [[UIActivityViewController alloc] initWithActivityItems:postItems applicationActivities:nil];
    [activityVc setValue:[NSString stringWithUTF8String:subjectLine] forKey:@"subject"];
    
    if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad && [activityVc respondsToSelector:@selector(popoverPresentationController)]) {
        UIPopoverController *popup = [[UIPopoverController alloc] initWithContentViewController:activityVc];
        
        [popup presentPopoverFromRect:CGRectMake([UIApplication sharedApplication].keyWindow.rootViewController.view.frame.size.width / 2,
                                                 [UIApplication sharedApplication].keyWindow.rootViewController.view.frame.size.height * arrowHeightPercentage, 0, 0)
                               inView:[UIApplication sharedApplication].keyWindow.rootViewController.view
             permittedArrowDirections:UIPopoverArrowDirectionDown
                             animated:YES];
    } else {
        [[UIApplication sharedApplication].keyWindow.rootViewController presentViewController:activityVc animated:YES completion:nil];
    }
}
