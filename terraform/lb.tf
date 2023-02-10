# Elastic IP

resource "aws_eip" "lb_eip" {
  vpc = true

  tags = {
    Name  = "${var.project_name}-eip"
    Owner = var.owner
  }
}

resource "aws_route53_record" "dns_dotnet" {
  zone_id = "${var.dns_hosted_zone_id}"
  name    = "${var.dns_record_name}"
  type    = "A"
  ttl     = 300
  records = [aws_eip.lb_eip.public_ip]
}

# Load Balancer

resource "aws_lb" "load_balancer" {
  name               = "${var.project_name}-lb"
  internal           = false
  load_balancer_type = "network"
  ip_address_type    = "ipv4"

  subnet_mapping {
    subnet_id     = "${module.vpc_subnet.public_subnet_ids[0]}"
    allocation_id = "${aws_eip.lb_eip.id}"
  }

  tags = {
    Name = "${var.project_name}-lb"
  }
}

resource "aws_lb_target_group" "target_group" {
  name        = "${var.project_name}-lb-tg-${substr(uuid(), 0, 3)}"
  port        = 80
  protocol    = "TCP"
  target_type = "ip"
  vpc_id      = module.vpc_subnet.vpc_id

  tags = {
    Name = "${var.project_name}-lb-tg"
  }

  lifecycle { 
    create_before_destroy = true
    ignore_changes = [name]
  }
}

resource "aws_lb_listener" "listener" {
  load_balancer_arn = aws_lb.load_balancer.id
  port              = "80"
  protocol          = "TCP"

  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.target_group.id
  }
}

resource "aws_lb_listener" "listener_443" {
  load_balancer_arn = aws_lb.load_balancer.id
  port              = "443"
  protocol          = "TLS"
  certificate_arn   = var.tls_certificate_arn
  alpn_policy       = "HTTP2Preferred"

  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.target_group.id
  }
}