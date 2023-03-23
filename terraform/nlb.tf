# Elastic IP

resource "aws_eip" "lb_eip" {
  #checkov:skip=CKV2_AWS_19:IP is being used for load balancer
  vpc = true
  count = "${var.num_public_subnets}"

  tags = {
    Name  = "${var.project_name}-eip-${count.index}"
    Owner = var.owner
  }
}

resource "aws_route53_record" "dns_dotnet" {
  zone_id = "${var.dns_hosted_zone_id}"
  name    = "${var.dns_record_name}"
  type    = "A"
  ttl     = 300
  records = aws_eip.lb_eip.*.public_ip
}

# Load Balancer

resource "aws_lb" "nlb" {
  #checkov:skip=CKV_AWS_150:Deletion protection is being left off for ease of running terraform destroy
  #checkov:skip=CKV2_AWS_20:Not required for our network load balancer, would be helpful for application load balancer instead
  #checkov:skip=CKV_AWS_152:We are leaving cross-zone load balancing disabled to save costs

  #checkov:skip=CKV_AWS_91:TODO: Add cloudwatch logging
  name                             = "${var.project_name}-nlb"
  internal                         = false
  load_balancer_type               = "network"
  ip_address_type                  = "ipv4"
  enable_cross_zone_load_balancing = false

  dynamic "subnet_mapping" {
    for_each = module.vpc_subnet.public_subnet_ids
    content {
      subnet_id     = "${subnet_mapping.value}"
      allocation_id = "${aws_eip.lb_eip[subnet_mapping.key].id}"
    }
  }

  tags = {
    Name = "${var.project_name}-nlb"
  }
}

resource "aws_lb_target_group" "nlb_target_group" {
  name        = "${var.project_name}-nlb-tg-${substr(uuid(), 0, 3)}"
  port        = 443
  protocol    = "TCP"
  target_type = "alb"
  vpc_id      = module.vpc_subnet.vpc_id

  tags = {
    Name = "${var.project_name}-nlb-tg"
  }

  lifecycle {
    create_before_destroy = true
    ignore_changes = [name]
  }
}

resource "aws_lb_listener" "nlb_listener" {
  load_balancer_arn = aws_lb.nlb.id
  port              = "80"
  protocol          = "TCP"

  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.nlb_target_group.id
  }
}

resource "aws_lb_listener" "nlb_listener_443" {
  load_balancer_arn = aws_lb.nlb.id
  port              = "443"
  protocol          = "TCP"

  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.nlb_target_group.id
  }
}