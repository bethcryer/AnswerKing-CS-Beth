resource "aws_wafv2_web_acl" "wafv2_alb_acl" {
    #checkov:skip=CKV2_AWS_31:TODO: Enable comprehensive logging using Amazon Kinesis Data Firehose and an S3 bucket to store logs

    name        = "${var.project_name}-acl"
    description = "Limits the rate at which one IP address can query the API, and blocks non-UK IP addresses"
    scope       = "REGIONAL"

    default_action {
        allow {}
    }

    rule {
        name     = "${var.project_name}-acl-rule-log4j2"
        priority = 1

        override_action {
            none {}
        }

        statement {
            managed_rule_group_statement {
                name        = "AWSManagedRulesKnownBadInputsRuleSet"
                vendor_name = "AWS"
            }
        }

        visibility_config {
            cloudwatch_metrics_enabled = true
            metric_name                = "${var.project_name}-acl-rule-log4j2-vis"
            sampled_requests_enabled   = false
        }
    }

    rule {
        name     = "${var.project_name}-acl-rule-region-lock"
        priority = 2

        action {
            block {}
        }

        statement {
            not_statement {
                statement {
                    geo_match_statement {
                        country_codes = ["GB"]
                    }
                }
            }
        }

        visibility_config {
            cloudwatch_metrics_enabled = true
            metric_name                = "${var.project_name}-acl-rule-regionlock-vis"
            sampled_requests_enabled   = false
        }
    }

    rule {
        name     = "${var.project_name}-acl-rule-ratelimit"
        priority = 3

        action {
            block {}
        }

        statement {
            rate_based_statement {
                limit              = 300
                aggregate_key_type = "IP"
            }
        }

        visibility_config {
            cloudwatch_metrics_enabled = true
            metric_name                = "${var.project_name}-acl-rule-ratelimit-vis"
            sampled_requests_enabled   = false
        }
    }

    visibility_config {
        cloudwatch_metrics_enabled = true
        metric_name                = "${var.project_name}-acl-vis"
        sampled_requests_enabled   = false
    }
}

resource "aws_wafv2_web_acl_association" "web_acl_association_my_lb" {
    resource_arn = aws_lb.alb.arn
    web_acl_arn  = aws_wafv2_web_acl.wafv2_alb_acl.arn
}