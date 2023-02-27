# AMI: Provides image info for Amazon Linux 2

data "aws_ami" "ecs_ami" {
  most_recent = true
  owners      = ["amazon"]

  filter {
    name   = "name"
    values = ["amzn2-ami-ecs-hvm-*-x86_64-ebs"]
  }
}

resource "aws_iam_instance_profile" "ecs_instance_profile" {
  name = "${var.project_name}-iam-instance-profile"
  role = aws_iam_role.ecs_task_role.name
}

# user_data script

data "template_file" "user_data" {
  template = file("${path.module}/scripts/user_data.sh")
  vars = {
    ECS_CLUSTER = "${var.project_name}-ecs-cluster"
  }
}

resource "aws_launch_configuration" "ecs_launch_config" {
    #checkov:skip=CKV_AWS_79:TODO: Disable the Instance Metadata Service or enable it with proper configuration (v2)
    #checkov:skip=CKV_AWS_8:TODO: Encrypt volume in future security ticket
    image_id             = data.aws_ami.ecs_ami.id
    iam_instance_profile = aws_iam_instance_profile.ecs_instance_profile.name
    security_groups      = [aws_security_group.ecs_sg.id]
    user_data            = data.template_file.user_data.rendered
    instance_type        = var.ec2_type

    lifecycle { 
      create_before_destroy = true
      ignore_changes = [name]
    }
}

resource "aws_autoscaling_group" "failure_analysis_ecs_asg" {
    name                      = "${var.project_name}-auto-scaling-group"
    launch_configuration      = aws_launch_configuration.ecs_launch_config.name
    vpc_zone_identifier       = [
      module.vpc_subnet.public_subnet_ids[0],
      module.vpc_subnet.public_subnet_ids[1]
    ]

    desired_capacity          = 1
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