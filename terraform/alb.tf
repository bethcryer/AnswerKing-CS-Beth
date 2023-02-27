# ALB Security Group

resource "aws_security_group" "alb_sg" {
  #checkov:skip=CKV_AWS_260:Allowing ingress from 0.0.0.0 for public HTTP(S) access
  name        = "${var.project_name}-alb-sg"
  description = "Security group for Application Load Balancer"
  vpc_id       = module.vpc_subnet.vpc_id

  ingress {
    from_port   = 80
    to_port     = 80
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
    description = "HTTP"
  }

  ingress {
    from_port   = 443
    to_port     = 443
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
    description = "HTTPS"
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
    description = "All traffic"
  }

  tags = {
    Name  = "${var.project_name}-alb-sg"
    Owner = var.owner
  }
}

resource "aws_lb" "alb" {
  #checkov:skip=CKV_AWS_150:Deletion protection is being left off for ease of running terraform destroy

  #checkov:skip=CKV_AWS_91:TODO: Add cloudwatch logging
  name               = "${var.project_name}-alb"
  internal           = false
  load_balancer_type = "application"
  subnets            = module.vpc_subnet.public_subnet_ids
  security_groups    = [aws_security_group.alb_sg.id]
  
  drop_invalid_header_fields = true

  tags = {
    Name = "${var.project_name}-alb"
  }
}

resource "aws_lb_target_group" "alb_target_group" {
  name        = "${var.project_name}-alb-tg-${substr(uuid(), 0, 3)}"
  port        = 80
  protocol    = "HTTP"
  target_type = "ip"
  vpc_id      = module.vpc_subnet.vpc_id

  health_check {
    healthy_threshold   = "3"
    interval            = "300"
    protocol            = "HTTP"
    matcher             = "200"
    timeout             = "3"
    path                = "/health"
    unhealthy_threshold = "2"
  }

  tags = {
    Name = "${var.project_name}-alb-tg"
  }

  lifecycle {
    create_before_destroy = true
    ignore_changes = [name]
  }
}

resource "aws_lb_listener" "alb_listener_https" {
  #checkov:skip=CKV_AWS_103: False positive, default SSL policy does enable TLS 1.2
  load_balancer_arn = aws_lb.alb.id
  port              = "443"
  protocol          = "HTTPS"
  certificate_arn   = var.tls_certificate_arn
  ssl_policy        = "ELBSecurityPolicy-2016-08"

  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.alb_target_group.id
  }
}

resource "aws_lb_target_group_attachment" "tg_attachment_80" {
    target_group_arn = aws_lb_target_group.nlb_target_group.arn
    target_id        = aws_lb.alb.arn
    port             = 443
}