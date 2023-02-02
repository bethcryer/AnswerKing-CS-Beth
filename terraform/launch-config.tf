# AMI: Provides image info for Amazon Linux 2

data "aws_ami" "ecs_ami" {
  most_recent = true
  owners      = ["amazon"]

  filter {
    name   = "name"
    values = ["amzn2-ami-ecs-hvm-*-x86_64-ebs"]
  }
}

# user_data script

data "template_file" "user_data" {
  template = file("${path.module}/scripts/user_data.sh")
  vars = {
    ECS_CLUSTER = "${var.project_name}-ecs-cluster"
  }
}

resource "aws_launch_configuration" "ecs_launch_config" {
    image_id             = data.aws_ami.ecs_ami.id
    iam_instance_profile = aws_iam_instance_profile.ecs_instance_profile.name
    security_groups      = [aws_security_group.ecs_sg.id]
    user_data            = data.template_file.user_data.rendered
    instance_type        = var.ec2_type
}

resource "aws_autoscaling_group" "failure_analysis_ecs_asg" {
    name                      = "${var.project_name}-auto-scaling-group"
    vpc_zone_identifier       = [module.vpc_subnet.public_subnet_ids[0]]
    launch_configuration      = aws_launch_configuration.ecs_launch_config.name

    desired_capacity          = 2
    min_size                  = 1
    max_size                  = 5
    health_check_grace_period = 300
    health_check_type         = "EC2"

    tag {
      key                 = "Name"
      value               = "${var.project_name}-ec2"
      propagate_at_launch = true
    }

    tag {
      key                 = "Owner"
      value               = var.owner
      propagate_at_launch = true
    }
}